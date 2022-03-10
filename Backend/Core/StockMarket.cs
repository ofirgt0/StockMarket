using System.Collections.Concurrent;
using Backend.Core;
using Backend.Entities;
using Backend.Helpers;
using Newtonsoft.Json;

namespace Backend
{
    public class StockMarket : IStockMarket
    {

        private const string BURSE_PLAYER_NAME = "Burse";
        private const int BURSE_INIT_AMOUNT = 0;
        private const int BURSE_INIT_ID = 0;

        private readonly object _sellersLocker;
        private readonly object _buyerLocker;
        public ConcurrentDictionary<string, Offer> BuyingOffers { get; set; }
        public ConcurrentDictionary<string, Offer> SellingOffers { get; set; }
        public List<Dealler> Deallers { get; set; }
        public Dictionary<string, List<Deal>> DeallersHistory { get; set; }
        public List<Stock> Stocks { get; set; }
        public Dictionary<string, Queue<Deal>> StocksHistory { get; set; }

        public StockMarket(IConfiguration configuration)
        {
            _sellersLocker = new object();
            _buyerLocker = new object();

            BuyingOffers = new ConcurrentDictionary<string, Offer>();
            SellingOffers = new ConcurrentDictionary<string, Offer>();
            Deallers = new List<Dealler>();
            Stocks = new List<Stock>();
            DeallersHistory = new Dictionary<string, List<Deal>>();
            StocksHistory = new Dictionary<string, Queue<Deal>>();

            var burseData = File.ReadAllText(configuration[Core.ConfigurationManager.CONSTANT + ":" + Core.ConfigurationManager.JSON_PATH]);
            BurseJsonDataHolder temporaryBurseDataObject = JsonConvert.DeserializeObject<BurseJsonDataHolder>(burseData);
            Deallers = temporaryBurseDataObject!.traders;
            Stocks = temporaryBurseDataObject.shares;

            DeallersHistory = Deallers.ToDictionary(element => element.Name, element => new List<Deal>());
            StocksHistory = Stocks.ToDictionary(element => element.Name, element => new Queue<Deal>());

            InitBursePlayer();
        }

        public void InitBursePlayer()
        {
            Dealler burse = new Dealler(BURSE_INIT_ID, BURSE_PLAYER_NAME, BURSE_INIT_AMOUNT);
            Deallers!.Add(burse);
            DeallersHistory!.TryAdd(burse.Name, new List<Deal>());
            foreach (Stock stock in Stocks)
            {
                SellingOffers.TryAdd(GenerateOfferName(BURSE_PLAYER_NAME, stock.Name), new Offer
                (burse, stock, stock.CurrentPrice, OfferType.sellingOffer, stock.Amount));
                AddStockToBuyerOwnedStocks(stock, burse, stock.Amount);
            }
        }

        public MakeADealResponse MakeADeal(string deallerName, string stockName, double wantedPrice, int wantedAmount, OfferType type)
        {
            Dealler owner = GetDeallerByName(deallerName);
            Stock retStock = GetStockByName(stockName);
            Offer toExcute = new Offer(owner, retStock, wantedPrice, type, wantedAmount);
            MakeADealResponse remainingAmount;

            remainingAmount = SearchAndExcute(toExcute);
            return remainingAmount;
        }

        public MakeADealResponse SearchAndExcute(Offer toExcute)
        {
            ConcurrentDictionary<string, Offer> toSearchOn = (toExcute.Type == OfferType.buyingOffer) ? SellingOffers : BuyingOffers;
            ConcurrentDictionary<string, Offer> toAddTo = (toExcute.Type == OfferType.buyingOffer) ? BuyingOffers : SellingOffers;
            var relevantLoker = (toExcute.Type == OfferType.buyingOffer) ? _sellersLocker : _buyerLocker;
            int oldAmount = toExcute.OfferStockAmount;

            if (!IsValidOffer(toExcute)) return new MakeADealResponse(toExcute.OfferStockAmount + 1, oldAmount);
            lock (relevantLoker)
            {
                IEnumerable<KeyValuePair<string, Offer>> filterOffers = toSearchOn
                .Where(offer => (offer.Value.Stock.Id == toExcute.Stock.Id &&
                toExcute.WantedPrice >= offer.Value.WantedPrice));
                if (filterOffers?.Any() ?? false)
                    RelevantOfferHandler(toExcute, filterOffers, toSearchOn);
            }

            if (toExcute.OfferStockAmount > 0)
                toAddTo.TryAdd(GenerateOfferName(toExcute.Owner.Name, toExcute.Stock.Name), toExcute);

            return new MakeADealResponse(toExcute.OfferStockAmount, oldAmount);
        }

        private void RelevantOfferHandler(Offer toExcute, IEnumerable<KeyValuePair<string, Offer>> filterOffers, ConcurrentDictionary<string, Offer> toSearchOn)
        {
            int diffrence = 0;
            double minPrice;
            foreach (var offer in filterOffers)
            {
                if (toExcute.OfferStockAmount == 0)
                    break;

                diffrence = Math.Min(toExcute.OfferStockAmount, offer.Value.OfferStockAmount);
                offer.Value.OfferStockAmount -= diffrence;
                toExcute.OfferStockAmount -= diffrence;
                if (offer.Value.OfferStockAmount == 0) toSearchOn.TryRemove(offer);
                minPrice = Math.Min(toExcute.WantedPrice, offer.Value.WantedPrice);
                TransferPropertiesBetweenDealers(offer.Value.Owner.Name, toExcute.Owner.Name, minPrice, diffrence, toExcute.Stock.Name);
                UpdateHistory(toExcute.Stock.Name, toExcute, offer.Value.Owner, false, diffrence, minPrice);
            }
        }

        public void TransferPropertiesBetweenDealers(string deallerSrcName, string deallerDstName, double moneyDifference, int amountDifference, string stockName)
        {
            Dealler srcDealler = GetDeallerByName(deallerSrcName);
            Dealler dstDealler = GetDeallerByName(deallerDstName);

            SetStockCurrentPriceByName(GetStockByName(stockName), moneyDifference);
            srcDealler.CurrMoney += moneyDifference * amountDifference;
            dstDealler.CurrMoney -= moneyDifference * amountDifference;
            srcDealler.OwnedStocksAmount -= Math.Abs(amountDifference);
            dstDealler.OwnedStocksAmount += Math.Abs(amountDifference);

            StockWithAmount relevantSrcStock = srcDealler.OwnedStocks.FirstOrDefault(s => s.Stock.Name == stockName);
            StockWithAmount relevantDstStock = dstDealler.OwnedStocks.FirstOrDefault(s => s.Stock.Name == stockName);

            if (relevantDstStock != null)
            {
                relevantSrcStock.decreaseAmountSafely(amountDifference);
                relevantDstStock.increaseAmountSafely(amountDifference);
            }
            else
            {
                relevantSrcStock.decreaseAmountSafely(amountDifference);
                dstDealler.OwnedStocks.Add(new StockWithAmount(relevantSrcStock.Stock, amountDifference));
            }
        }

        public void SetStockCurrentPriceByName(Stock stock, double price)
        {
            double percentageDifference = (stock.CurrentPrice > price) ? (stock.CurrentPrice / price) : ((price / stock.CurrentPrice));
            stock.CurrentPrice = price;
            stock.PercentageDifference.Push(Math.Round(percentageDifference, 2));
        }

        public void UpdateHistory(string stockName, Offer offerToAdd, Dealler dealler, bool deallerIsBuyer, int amount, double price)
        {
            Deal dealToAdd;
            if (!(offerToAdd.Type == OfferType.buyingOffer))
                dealToAdd = new Deal(stockName, offerToAdd.Owner.Name, dealler.Name, price, amount);
            else
                dealToAdd = new Deal(stockName, dealler.Name, offerToAdd.Owner.Name, price, amount);

            DeallersHistory[dealler.Name].Add(dealToAdd);
            DeallersHistory[offerToAdd.Owner.Name].Add(dealToAdd);

            StocksHistory[offerToAdd.Stock.Name].Enqueue(dealToAdd);
        }

        public void AddStockToBuyerOwnedStocks(Stock stockToAdd, Dealler dealler, int amount)
        {
            StockWithAmount temporaryStockToCheck = dealler.OwnedStocks.FirstOrDefault
            (dealllerOwnedStock => dealllerOwnedStock.Stock.Id == stockToAdd.Id);

            if (temporaryStockToCheck != null) temporaryStockToCheck.increaseAmountSafely(amount);
            else dealler.OwnedStocks.Add(new StockWithAmount(stockToAdd, amount));
            dealler.OwnedStocksAmount += amount;
        }

        public bool InsertOffer(string deallerName, string stockName, double wantedPrice, int amount, string type)
        {
            Offer toAdd = BuildOffer(deallerName, stockName, wantedPrice, amount, type);
            if (!IsValidOffer(toAdd))
                return false;

            if (toAdd.Type == OfferType.buyingOffer)
                BuyingOffers.TryAdd(GenerateOfferName(toAdd.Owner.Name, toAdd.Stock.Name), toAdd);
            else
                SellingOffers.TryAdd(GenerateOfferName(toAdd.Owner.Name, toAdd.Stock.Name), toAdd);
            return true;
        }

        private Offer BuildOffer(string deallerName, string stockName, double wantedPrice, int amount, string type)
        {
            Dealler activeDealler = GetDeallerByName(deallerName);
            Stock SellingOffertock = GetStockByName(stockName);
            OfferType offerType = (type == "buyingOffer") ? OfferType.buyingOffer : OfferType.sellingOffer;
            return new Offer(activeDealler, SellingOffertock, wantedPrice, offerType, amount);
        }

        public string GenerateOfferName(string ownerName, string stockName)
        {
            return ownerName + " - " + stockName;
        }

        public Dealler GetDeallerByName(string deallerName)
        {
            return Deallers.FirstOrDefault(d => d.Name == deallerName);
        }

        public Stock GetStockByName(string stockName)
        {
            return Stocks.FirstOrDefault(d => d.Name == stockName);
        }

        public bool IsValidOffer(Offer toCheck)
        {
            var OfferName = GenerateOfferName(toCheck.Owner.Name, toCheck.Stock.Name);
            StockWithAmount generateObjToCheck = new StockWithAmount(toCheck.Stock, toCheck.OfferStockAmount);
            if ((toCheck.Type == OfferType.buyingOffer))
            {
                if (toCheck.Owner.CurrMoney < toCheck.WantedPrice)
                {
                    Console.WriteLine(toCheck.Owner.Name + " dosen't have enoght money");
                    return false;
                }

                if (SellingOffers.ContainsKey(OfferName))
                {
                    Console.WriteLine(toCheck.Owner.Name + " already have en exist selling offer");
                    return false;
                }
            }

            else if (toCheck.Type == OfferType.sellingOffer)
            {
                if (toCheck.Owner.OwnedStocks.FirstOrDefault(stock => generateObjToCheck.Stock.Name == stock.Stock.Name &&
                 generateObjToCheck.Amount < stock.Amount) == null)
                {
                    Console.WriteLine(toCheck.Owner.Name + " dosen't have the relevant stock");
                    return false;
                }

                if (BuyingOffers.ContainsKey(OfferName))
                {
                    Console.WriteLine(toCheck.Owner.Name + " already have en exist buying offer");
                    return false;
                }
            }

            return true;
        }

        public Stock GetStockById(int id)
        {
            return Stocks.FirstOrDefault(d => d.Id == id);
        }

        public Dealler GetDeallerById(int id)
        {
            return Deallers.FirstOrDefault(d => d.Id == id);
        }
        public void UpdateOffers()
        {
            double latestStockValueUpdate;
            lock (_buyerLocker)
            {
                foreach (KeyValuePair<string, Offer> offer in BuyingOffers)
                {
                    latestStockValueUpdate = offer.Value.Stock.PercentageDifference.Peek();
                    offer.Value.WantedPrice += (latestStockValueUpdate * offer.Value.WantedPrice) / 100;
                }
            }

            lock (_sellersLocker)
            {
                foreach (KeyValuePair<string, Offer> offer in SellingOffers)
                {
                    latestStockValueUpdate = offer.Value.Stock.PercentageDifference.Peek();
                    offer.Value.WantedPrice += (latestStockValueUpdate * offer.Value.WantedPrice) / 100;
                }
            }
        }

        public List<Deal> GetDeallerDeals(string name)
        {
            return DeallersHistory[name];
        }

        public HoldingsWorth GetDeallersWorth(string name)
        {
            Dealler dealler = GetDeallerByName(name);
            HoldingsWorth holdings = new HoldingsWorth(0,dealler.MoneyAtOpening,dealler.CurrMoney);
            double totalHoldingsWorth=0;
            foreach (var stockWithAmount in dealler.OwnedStocks)
            {
                totalHoldingsWorth += stockWithAmount.Amount * stockWithAmount.Stock.CurrentPrice;
                holdings.OwnedStockWorth.TryAdd(stockWithAmount.Stock.Name,stockWithAmount.Stock.CurrentPrice*stockWithAmount.Amount);
            }
            holdings.TotalWorth=totalHoldingsWorth;
            return holdings;
        }

    }
}
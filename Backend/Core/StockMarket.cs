using System.Collections.Concurrent;
using Backend.Core;
using Backend.Entities;
using Backend.Helpers;
using static Backend.Helpers.OfferTypeClass;
using Newtonsoft.Json;

namespace Backend
{
    public class StockMarket : IStockMarket
    {
        //private readonly IConfiguration Configuration;

        
        private const string BURSE_PLAYER_NAME = "Burse";
        private const int BURSE_INIT_AMOUNT = 0;
        private const int BURSE_INIT_ID = 0;

        private readonly object _sellersLocker;
        private readonly object _buyerLocker;
        public ConcurrentDictionary<string, Offer> BuyingOffers { get; set; }
        public ConcurrentDictionary<string, Offer> SellingOffers { get; set; }
        public List<Dealler> Deallers { get; set; }
        public Dictionary<string, Queue<Deal>> DeallersHistory { get; set; }
        public List<Stock> Stocks { get; set; }
        public Dictionary<string, Queue<Deal>> StocksHistory { get; set; }

        public StockMarket(IConfiguration configuration)
        {
            _sellersLocker = new object();
            _buyerLocker=new object();

            BuyingOffers = new ConcurrentDictionary<string, Offer>();
            SellingOffers = new ConcurrentDictionary<string, Offer>();
            Deallers = new List<Dealler>();
            Stocks = new List<Stock>();
            DeallersHistory = new Dictionary<string, Queue<Deal>>();
            StocksHistory = new Dictionary<string, Queue<Deal>>();

            var burseData = File.ReadAllText(configuration[Core.ConfigurationManager.CONSTANT + ":" + Core.ConfigurationManager.JSON_PATH]);
            BurseJsonDataHolder temporaryBurseDataObject = JsonConvert.DeserializeObject<BurseJsonDataHolder>(burseData);
            Deallers = temporaryBurseDataObject!.traders;
            Stocks = temporaryBurseDataObject.shares;

            DeallersHistory=Deallers.ToDictionary(element=>element.Name,element=>new Queue<Deal>());
            StocksHistory=Stocks.ToDictionary(element=>element.Name,element=>new Queue<Deal>());
            
            InitBursePlayer();
        }
        public void InitBursePlayer()
        {
            Dealler burse = new Dealler(BURSE_INIT_ID, BURSE_PLAYER_NAME, BURSE_INIT_AMOUNT);
            Deallers!.Add(burse);
            DeallersHistory!.TryAdd(burse.Name, new Queue<Deal>());
            foreach (Stock stock in Stocks)
            {
                SellingOffers.TryAdd(GenerateOfferName(BURSE_PLAYER_NAME,stock.Name), new Offer
                (burse, stock, stock.CurrentPrice, OfferType.sellingOffer, stock.Amount));
                AddStockToBuyerOwnedStocks(stock, burse, stock.Amount);
            }
        }

        public MakeADealResponse MakeADeal(string deallerName, string stockName, double wantedPrice, int wantedAmount, OfferType type)
        {
            Dealler owner=GetDeallerByName(deallerName);
            Stock retStock=GetStockByName(stockName);
            Offer toExcute = new Offer(owner, retStock, wantedPrice, type, wantedAmount);
            MakeADealResponse remainingAmount;

            remainingAmount = SearchAndExcute(toExcute);
            SetStockCurrentPriceByName(toExcute.Stock, toExcute.WantedPrice);

            return remainingAmount;
        }
    
        public MakeADealResponse SearchAndExcute(Offer toExcute)
        {
            ConcurrentDictionary<string, Offer> toSearchOn = (toExcute.Type == OfferType.buyingOffer) ? SellingOffers : BuyingOffers;
            var relevantLoker=(toExcute.Type == OfferType.buyingOffer) ? _sellersLocker:_buyerLocker;
            int oldAmount = toExcute.OfferStockAmount;
            
            lock (relevantLoker)
            {
                IEnumerable<KeyValuePair<string, Offer>> filterOffers = toSearchOn
                .Where(offer => (offer.Value.Stock.Id == toExcute.Stock.Id &&
                toExcute.WantedPrice >= offer.Value.WantedPrice));
                if (filterOffers != null)
                    RelevantOfferHandler(toExcute,filterOffers,toSearchOn);   
            }

            if (toExcute.OfferStockAmount > 0)
            {
                BuyingOffers.TryAdd(GenerateOfferName(toExcute.Owner.Name, toExcute.Stock.Name), toExcute);
            }
            return new MakeADealResponse(toExcute.OfferStockAmount, oldAmount);
        }
        private void RelevantOfferHandler(Offer toExcute,IEnumerable<KeyValuePair<string, Offer>> filterOffers,ConcurrentDictionary<string, Offer> toSearchOn)
        {
            int diffrence=0;
            foreach (var offer in filterOffers)
            {
                if (toExcute.OfferStockAmount == 0)
                    break;
                
                diffrence = Math.Abs(toExcute.OfferStockAmount - offer.Value.OfferStockAmount);
                offer.Value.OfferStockAmount -= diffrence;
                toExcute.OfferStockAmount-=diffrence;
                if(offer.Value.OfferStockAmount==0) toSearchOn.TryRemove(offer); 

                TransferPropertiesBetweenDealers(offer.Value.Owner.Name, toExcute.Owner.Name, Math.Min(toExcute.WantedPrice, offer.Value.WantedPrice), toExcute.OfferStockAmount, toExcute.Stock.Name);
                UpdateHistory(toExcute, offer.Value.Owner, false);
                toExcute.OfferStockAmount = 0;
            }
        }
        public void TransferPropertiesBetweenDealers(string deallerSrcName, string deallerDstName, double moneyDifference, int amountDifference, string stockName)
        {
            Dealler srcDealler = GetDeallerByName(deallerSrcName);
            Dealler dstDealler = GetDeallerByName(deallerDstName);

            srcDealler.CurrMoney += moneyDifference * amountDifference;
            srcDealler.OwnedStocksAmount-=Math.Abs(amountDifference);
            dstDealler.CurrMoney -= moneyDifference * amountDifference;
            dstDealler.OwnedStocksAmount+=amountDifference;
            
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

        public void RemoveOffer(Offer toRemove)
        {
            if (toRemove.Type == OfferType.buyingOffer)
                BuyingOffers.Remove(GenerateOfferName(toRemove.Owner.Name, toRemove.Stock.Name), out _);
            else
                SellingOffers.Remove(GenerateOfferName(toRemove.Owner.Name, toRemove.Stock.Name), out _);
        }

        public void SetStockCurrentPriceByName(Stock stock, double price)
        {
            double percentageDifference = (stock.CurrentPrice > price) ? (stock.CurrentPrice / price) : (-1 * (price / stock.CurrentPrice));
            stock.CurrentPrice = price;
            stock.PercentageDifference.Push(percentageDifference);
        }

        public void UpdateHistory(Offer offerToAdd, Dealler dealler, bool deallerIsBuyer)
        {
            Deal deal;
            if (!deallerIsBuyer)
                deal = new Deal(offerToAdd.Owner, dealler, offerToAdd.WantedPrice, offerToAdd.OfferStockAmount);
            else
                deal = new Deal(dealler, offerToAdd.Owner, offerToAdd.WantedPrice, offerToAdd.OfferStockAmount);

            DeallersHistory[dealler.Name].Enqueue(deal);
            DeallersHistory[offerToAdd.Owner.Name].Enqueue(deal);

            StocksHistory[offerToAdd.Stock.Name].Enqueue(deal);
        }

        public void AddStockToBuyerOwnedStocks(Stock stockToAdd, Dealler dealler, int amount)
        {
            StockWithAmount temporaryStockToCheck = dealler.OwnedStocks.FirstOrDefault(d => d.Stock.Id == stockToAdd.Id);
            if (temporaryStockToCheck != null) temporaryStockToCheck.increaseAmountSafely(amount);

            else dealler.OwnedStocks.Add(new StockWithAmount(stockToAdd, amount));
            dealler.OwnedStocksAmount+=amount;
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
        public Offer GetOfferByName(string offerName)
        {
            Offer res;
            if (BuyingOffers.TryGetValue(offerName, out res!) || SellingOffers.TryGetValue(offerName, out res!))
                return res;
            return null!;
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
            var a = GenerateOfferName(toCheck.Owner.Name, toCheck.Stock.Name);
            StockWithAmount generateObjToCheck=new StockWithAmount(toCheck.Stock,toCheck.OfferStockAmount);
            if(toCheck.Type==OfferType.buyingOffer&&toCheck.Owner.CurrMoney<toCheck.WantedPrice)
                return false;
            else if(!toCheck.Owner.OwnedStocks.Contains(generateObjToCheck))
                return false;
            return !BuyingOffers.ContainsKey(a) &&
                    !SellingOffers.ContainsKey(a);
        }
        public Stock GetStockById(int id)
        {
            return Stocks.FirstOrDefault(d => d.Id == id);
        }
        public Dealler GetDeallerById(int id)
        {
            return Deallers.FirstOrDefault(d => d.Id == id);
        }
        public void UpdateOffers(){
            double latestStockValueUpdate;
            lock(_buyerLocker)
            {
                foreach (KeyValuePair<string, Offer> offer in BuyingOffers)
                {
                    latestStockValueUpdate = offer.Value.Stock.PercentageDifference.Peek();
                    offer.Value.WantedPrice += (latestStockValueUpdate * offer.Value.WantedPrice) / 100;
                }
            }
            lock(_sellersLocker)
            {
                foreach (KeyValuePair<string, Offer> offer in SellingOffers)
                {
                    latestStockValueUpdate = offer.Value.Stock.PercentageDifference.Peek();
                    offer.Value.WantedPrice += (latestStockValueUpdate * offer.Value.WantedPrice) / 100;
                }
            }
        }
    }
}
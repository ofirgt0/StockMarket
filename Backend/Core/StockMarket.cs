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

        private const string JSON_PATH = "JSON_PATH";
        private readonly object _sellersLocker;
        public ConcurrentDictionary<string, Offer> BuyingOffer { get; set; }
        public ConcurrentDictionary<string, Offer> SellingOffer { get; set; }
        public List<Dealler> Deallers { get; set; }
        public Dictionary<string, Queue<Deal>> DeallersHistory { get; set; }
        public List<Stock> Stocks { get; set; }
        public Dictionary<string, Queue<Deal>> StocksHistory { get; set; }

        public StockMarket(IConfiguration configuration)
        {
            // Configuration = configuration;
            _sellersLocker = new object();

            BuyingOffer = new ConcurrentDictionary<string, Offer>();
            SellingOffer = new ConcurrentDictionary<string, Offer>();
            Deallers = new List<Dealler>();
            Stocks = new List<Stock>();
            DeallersHistory = new Dictionary<string, Queue<Deal>>();
            StocksHistory = new Dictionary<string, Queue<Deal>>();

            var burseData = File.ReadAllText(configuration["CONSTANT:" + JSON_PATH]);
            BurseJsonDataHolder temporaryBurseDataObject = JsonConvert.DeserializeObject<BurseJsonDataHolder>(burseData);
            Deallers = temporaryBurseDataObject!.traders;
            Stocks = temporaryBurseDataObject.shares;

            for (int i = 0; i < Deallers.Count(); i++)
            {
                DeallersHistory!.TryAdd(Deallers.ElementAt(i).Name, new Queue<Deal>());
            }

            for (int i = 0; i < Stocks.Count(); i++)
            {
                StocksHistory!.TryAdd(Stocks.ElementAt(i).Name, new Queue<Deal>());
            }

            InitBursePlayer();
        }
        public void InitBursePlayer()
        {
            Dealler burse = new Dealler(0, "Burse", 0);
            Deallers!.Add(burse);
            DeallersHistory!.TryAdd(burse.Name, new Queue<Deal>());
            foreach (Stock stock in Stocks)
            {
                SellingOffer.TryAdd("Burse - " + stock.Name, new Offer
                (burse, stock, stock.CurrentPrice, OfferType.sellingOffer, stock.Amount));
                AddStockToBuyerOwnedStocks(stock, burse, stock.Amount);
            }
        }

        public MakeADealResponse MakeADeal(string deallerName, string stockName, double wantedPrice, int wantedAmount, OfferType type)
        {
            Offer toExcute = new Offer(GetDeallerByName(deallerName), GetStockByName(stockName), wantedPrice, type, wantedAmount);
            MakeADealResponse remainingAmount;

            remainingAmount = toExcute.Type == OfferType.buyingOffer ? 
                                SearchAndExcuteBuyingOffer(toExcute) : SearchAndExcuteSellingOffer(toExcute);

            RemoveEmptyOffers();
            SetStockCurrentPriceByName(toExcute.Stock, toExcute.WantedPrice);

            return remainingAmount;
        }
        public void RemoveEmptyOffers()
        {
            foreach (var offer in BuyingOffer)
            {
                if (offer.Value.OfferStockAmount == 0)
                    BuyingOffer.Remove(offer.Key, out _);
            }
            foreach (var offer in SellingOffer)
            {
                if (offer.Value.OfferStockAmount == 0)
                    SellingOffer.Remove(offer.Key, out _);
            }
        }
        public MakeADealResponse SearchAndExcuteBuyingOffer(Offer toExcute)
        {
            int oldAmount = toExcute.OfferStockAmount;
            lock (_sellersLocker)
            {
                IEnumerable<KeyValuePair<string, Offer>> filterOffers = SellingOffer
                .Where(offer => (offer.Value.Stock.Id == toExcute.Stock.Id &&
                toExcute.WantedPrice >= offer.Value.WantedPrice));
                if (filterOffers != null)
                {
                    foreach (var offer in filterOffers)
                    {
                        if (toExcute.OfferStockAmount == 0)
                            break;

                        if (toExcute.OfferStockAmount < offer.Value.OfferStockAmount)
                        {
                            offer.Value.OfferStockAmount -= toExcute.OfferStockAmount;
                            TransferPropertiesBetweenDealers(offer.Value.Owner.Name, toExcute.Owner.Name, Math.Min(toExcute.WantedPrice, offer.Value.WantedPrice), toExcute.OfferStockAmount, toExcute.Stock.Name);
                            UpdateHistory(toExcute, offer.Value.Owner, false);
                            toExcute.OfferStockAmount = 0;
                        }
                        else
                        {
                            toExcute.OfferStockAmount -= offer.Value.OfferStockAmount;
                            TransferPropertiesBetweenDealers(offer.Value.Owner.Name, toExcute.Owner.Name, Math.Min(toExcute.WantedPrice, offer.Value.WantedPrice), offer.Value.OfferStockAmount, toExcute.Stock.Name);
                            UpdateHistory(toExcute, offer.Value.Owner, false);
                            offer.Value.OfferStockAmount = 0;
                        }
                    }
                }
            }
            if (toExcute.OfferStockAmount > 0)
            {
                BuyingOffer.TryAdd(GenerateOfferName(toExcute.Owner.Name, toExcute.Stock.Name), toExcute);
            }
            return new MakeADealResponse(toExcute.OfferStockAmount, oldAmount);
        }
        public MakeADealResponse SearchAndExcuteSellingOffer(Offer toExcute)
        {
            int oldAmount = toExcute.OfferStockAmount;
            lock (BuyingOffer)
            {
                IEnumerable<KeyValuePair<string, Offer>> filterOffers = BuyingOffer
                .Where(offer => (offer.Value.Stock.Id == toExcute.Stock.Id &&
                toExcute.WantedPrice <= offer.Value.WantedPrice));
                foreach (var offer in filterOffers)
                {
                    if (toExcute.OfferStockAmount == 0)
                        break;
                    if (toExcute.OfferStockAmount < offer.Value.OfferStockAmount)
                    {
                        offer.Value.OfferStockAmount -= toExcute.OfferStockAmount;
                        TransferPropertiesBetweenDealers(toExcute.Owner.Name, offer.Value.Owner.Name, Math.Min(toExcute.WantedPrice, offer.Value.WantedPrice), toExcute.OfferStockAmount, toExcute.Stock.Name);
                        UpdateHistory(toExcute, offer.Value.Owner, true);
                        toExcute.OfferStockAmount = 0;
                    }
                    else
                    {
                        toExcute.OfferStockAmount -= offer.Value.OfferStockAmount;
                        TransferPropertiesBetweenDealers(toExcute.Owner.Name, offer.Value.Owner.Name, Math.Min(toExcute.WantedPrice, offer.Value.WantedPrice), offer.Value.OfferStockAmount, toExcute.Stock.Name);
                        UpdateHistory(toExcute, offer.Value.Owner, true);
                        offer.Value.OfferStockAmount = 0;
                    }
                }
            }
            if (toExcute.OfferStockAmount > 0)
            {
                SellingOffer.TryAdd(GenerateOfferName(toExcute.Owner.Name, toExcute.Stock.Name), toExcute);
            }
            return new MakeADealResponse(toExcute.OfferStockAmount, oldAmount);
        }
        public void TransferPropertiesBetweenDealers(string deallerSrcName, string deallerDstName, double moneyDifference, int amountDifference, string stockName)
        {
            Dealler srcDealler = GetDeallerByName(deallerSrcName);
            Dealler dstDealler = GetDeallerByName(deallerDstName);

            srcDealler.CurrMoney += moneyDifference * amountDifference;
            dstDealler.CurrMoney -= moneyDifference * amountDifference;
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
                BuyingOffer.Remove(GenerateOfferName(toRemove.Owner.Name, toRemove.Stock.Name), out _);
            else
                SellingOffer.Remove(GenerateOfferName(toRemove.Owner.Name, toRemove.Stock.Name), out _);
        }
        public bool IsValidDeal(Dealler dealler, Offer offerToCheck)
        {
            if (offerToCheck.Type == OfferType.sellingOffer &&
            offerToCheck.Owner.OwnedStocks.FirstOrDefault(s => s.Stock.Name == offerToCheck.Stock.Name) != null)
                return (dealler.CurrMoney > offerToCheck.WantedPrice);

            if (offerToCheck.Type == OfferType.buyingOffer)
                return (dealler.OwnedStocks.FirstOrDefault(s => s.Stock.Name == offerToCheck.Stock.Name) != null);
            return false;
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
        }
        public bool InsertOffer(Offer toAdd)
        {
            if (!IsValidOffer(toAdd))
                return false;
            if (toAdd.Type == OfferType.buyingOffer)
                BuyingOffer.TryAdd(GenerateOfferName(toAdd.Owner.Name, toAdd.Stock.Name), toAdd);
            else
                SellingOffer.TryAdd(GenerateOfferName(toAdd.Owner.Name, toAdd.Stock.Name), toAdd);
            return true;
        }
        public string GenerateOfferName(string ownerName, string stockName)
        {
            return ownerName + " - " + stockName;
        }
        public Offer GetOfferByName(string offerName)
        {
            Offer res;
            if (BuyingOffer.TryGetValue(offerName, out res!) || SellingOffer.TryGetValue(offerName, out res!))
                return res;
            return null!;
        }
        public Dealler GetDeallerByName(string deallerName)
        {
            return this.Deallers.FirstOrDefault(d => d.Name == deallerName);
        }
        public Stock GetStockByName(string stockName)
        {
            return this.Stocks.FirstOrDefault(d => d.Name == stockName);
        }
        public bool IsValidOffer(Offer toCheck)
        {
            var a = GenerateOfferName(toCheck.Owner.Name, toCheck.Stock.Name);
            return !this.BuyingOffer.ContainsKey(a) &&
                    !this.SellingOffer.ContainsKey(a);
        }
        public Stock GetStockById(int id)
        {
            return Stocks.FirstOrDefault(d => d.Id == id);
        }
        public Dealler GetDeallerById(int id)
        {
            return this.Deallers.FirstOrDefault(d => d.Id == id);
        }
    }
}
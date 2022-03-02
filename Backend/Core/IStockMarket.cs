using System.Collections.Concurrent;
using Backend.Entities;
using Backend.Helpers;
using static Backend.Helpers.OfferTypeClass;

namespace Backend.Core
{
    public interface IStockMarket
    {
        public ConcurrentDictionary<string, Offer> BuyingOffers { get; set; }
        public ConcurrentDictionary<string, Offer> SellingOffers { get; set; }
        public List<Dealler> Deallers { get; set; }
        public Dictionary<string, Queue<Deal>> DeallersHistory { get; set; }
        public List<Stock> Stocks { get; set; }
        public Dictionary<string, Queue<Deal>> StocksHistory { get; set; }

        public void InitBursePlayer();

        public bool InsertOffer(string deallerName, string stockName, double wantedPrice, int amount, string type);

        public void RemoveOffer(Offer toRemove);

        public Offer GetOfferByName(string offerName);

        public Dealler GetDeallerByName(string deallerName);

        public Stock GetStockByName(string stockName);

        public void TransferPropertiesBetweenDealers(string deallerSrcName, string deallerDstName, double moneyDifference, int amountDifference, string stockName);

        public MakeADealResponse MakeADeal(string deallerName, string stockName, double wantedPrice, int wantedAmount, OfferType type);

        public void UpdateHistory(Offer offerToAdd, Dealler dealler, bool deallerIsBuyer);

        public Stock GetStockById(int id);
        public Dealler GetDeallerById(int id);
        public void UpdateOffers();

    }
}
using System.Collections.Concurrent;
using Backend.Entities;
using Backend.Helpers;
using static Backend.Helpers.OfferTypeClass;

namespace Backend.Core
{
    public interface IStockMarket
    {
        public ConcurrentDictionary<string, Offer> BuyingOffer { get; set; }
        public ConcurrentDictionary<string, Offer> SellingOffer { get; set; }
        public List<Dealler> Deallers { get; set; }
        public Dictionary<string, Queue<Deal>> DeallersHistory { get; set; }
        public List<Stock> Stocks { get; set; }
        public Dictionary<string, Queue<Deal>> StocksHistory { get; set; }

        public void InitBursePlayer();

        public bool InsertOffer(Offer toAdd);

        public void RemoveOffer(Offer toRemove);

        public bool IsValidOffer(Offer toCheck);

        public Offer GetOfferByName(string offerName);

        public Dealler GetDeallerByName(string deallerName);

        public Stock GetStockByName(string stockName);

        public void TransferPropertiesBetweenDealers(string deallerSrcName, string deallerDstName, double moneyDifference, int amountDifference, string stockName);

        public MakeADealResponse MakeADeal(string deallerName, string stockName, double wantedPrice, int wantedAmount, OfferType type);

        public void UpdateHistory(Offer offerToAdd, Dealler dealler, bool deallerIsBuyer);

        public bool IsValidDeal(Dealler dealler, Offer offerToCheck);
        public Stock GetStockById(int id);
        public Dealler GetDeallerById(int id);

    }
}
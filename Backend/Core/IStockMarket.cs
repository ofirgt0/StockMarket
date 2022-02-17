using System.Collections.Concurrent;
using Backend.Entities;

namespace Backend.Core
{
    public interface IStockMarket
    {
        public ConcurrentDictionary<string, Offer> OffersB { get; set; }//list of buyers - <(offer's owner name)-(stock name),offer>

        public ConcurrentDictionary<string, Offer> OffersS { get; set; }

        public List<Dealler> Deallers { get; set; }

        public Dictionary<string, Queue<Deal>> DeallersHistory { get; set; }

        public List<Stock> Stocks { get; set; }

        public Dictionary<string, Queue<Deal>> StocksHistory { get; set; }

        public Boolean insertOffer(Offer toAdd);

        public void initBursePlayer();

        public void initEmptyFields();

        public Boolean isValidOffer(Offer toAdd);

        public Offer getOfferByName(string offerName);

        public Dealler getDeallerByName(string deallerName);

        public Stock getStockByName(string stockName);

        public void transferMoneyBetweenDealers(string deallerSrc, string deallerDst, double difference);

        public void setStockCurrentPriceByName(string stockName, double price);

        public void decreaseStockAmountInBurseByName(string stockName, int difference);

        public Boolean makeADeal(string offerName, string deallerName);

        public void addStockToBuyerOwnedStocks(Stock stockToAdd, Dealler dealler);

        public void updateHistory(Offer offerToAdd, Dealler dealler, Boolean deallerIsBuyer);

        public Boolean isValidDeal(Dealler dealler, Offer offerToCheck);
    }
}
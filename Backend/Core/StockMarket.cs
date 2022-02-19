using System.Collections.Concurrent;
using Backend.Core;
using Backend.Entities;
using Backend.Helpers;
using Newtonsoft.Json.Linq;
using static Backend.Helpers.OfferTypeClass;

namespace Backend
{
    public class StockMarket : IStockMarket
    {
        public ConcurrentDictionary<string, Offer> OffersB { get; set; }//list of buyers - <(offer's owner name)-(stock name),offer>
        public ConcurrentDictionary<string, Offer> OffersS { get; set; }
        public List<Dealler> Deallers { get; set; }
        public Dictionary<string, Queue<Deal>> DeallersHistory { get; set; }
        public List<Stock> Stocks { get; set; }
        public Dictionary<string, Queue<Deal>> StocksHistory { get; set; }

        public StockMarket()
        {
            initEmptyFields();
            var burseData = File.ReadAllText("C:/angular/stockMarket/Backend/Entities/BurseJson.json");
            var deallers = JObject.Parse(burseData)["traders"];
            var stocks = JObject.Parse(burseData)["shares"];

            Dealler newDeallerFromJson;
            for (int i = 0; i < deallers!.Count(); i++)
            {
                newDeallerFromJson = new Dealler((int)deallers![i]!["id"]!, (string)deallers[i]!["name"]!, (int)deallers[i]!["money"]!);
                Deallers!.Add(newDeallerFromJson);
                DeallersHistory!.TryAdd(newDeallerFromJson.Name, new Queue<Deal>());
            }

            Stock newStockFromJson;
            for (int i = 0; i < stocks!.Count(); i++)
            {
                newStockFromJson = new Stock((int)stocks![i]!["id"]!, (string)stocks[i]!["name"]!, (int)stocks[i]!["currentPrice"]!, (int)stocks[i]!["amount"]!);
                Stocks!.Add(newStockFromJson);
                StocksHistory!.TryAdd(newStockFromJson.Name, new Queue<Deal>());
            }
            initBursePlayer();
        }
        public void initBursePlayer()
        {
            Dealler burse = new Dealler(0, "Burse", 0);
            Deallers!.Add(burse);
            DeallersHistory!.TryAdd(burse.Name, new Queue<Deal>());
            foreach (Stock stock in Stocks)
            {
                OffersS.TryAdd("Burse - " + stock.Name, new Offer
                (burse, stock, stock.CurrentPrice, OfferType.sellingOffer, stock.CurrentStockAmountInBurse));
                addStockToBuyerOwnedStocks(stock, burse, stock.Amount);
            }
        }
        public void initEmptyFields()
        {
            OffersB = new ConcurrentDictionary<string, Offer>();
            OffersS = new ConcurrentDictionary<string, Offer>();
            Deallers = new List<Dealler>();
            Stocks = new List<Stock>();
            DeallersHistory = new Dictionary<string, Queue<Deal>>();
            StocksHistory = new Dictionary<string, Queue<Deal>>();
        }
        
        //make a deal 
        public Boolean makeADeal(string offerName, string deallerName)
        {
            Offer offerToExecut = getOfferByName(offerName);
            Dealler dealler = getDeallerByName(deallerName);
            Dealler buyer, seller;

            if (offerToExecut.Type == OfferType.sellingOffer)
            {
                buyer = getDeallerByName(deallerName);
                seller = offerToExecut.Owner;
            }
            else
            {
                seller = getDeallerByName(deallerName);
                buyer = offerToExecut.Owner;
            }

            if (isValidDeal(dealler, offerToExecut))
            {
                //TODO: remove stock from seller, remove offer
                transferPropBetweenDealers(seller.Name,buyer.Name, offerToExecut.WantedPrice, offerToExecut.Amount, offerToExecut.Stock.Name);
                
                removeOffer(offerToExecut);

                setStockCurrentPriceByName(offerToExecut.Stock.Name, offerToExecut.WantedPrice);

                updateHistory(offerToExecut, dealler, true);
                return true;
            }
            else
                return false;
        }
        public void transferPropBetweenDealers(string deallerSrcName, string deallerDstName, double moneyDifference, int amountDifference, string stockName)
        {
            Dealler srcDealler = getDeallerByName(deallerSrcName);
            Dealler dstDealler = getDeallerByName(deallerDstName);

            srcDealler.CurrMoney += moneyDifference;
            dstDealler.CurrMoney -= moneyDifference;
            
            if(dstDealler.OwnedStocks.FirstOrDefault(s => s.Stock.Name == stockName,null)!=null)
            {
                srcDealler.OwnedStocks.FirstOrDefault(s => s.Stock.Name == stockName)!.Amount -= amountDifference;
                dstDealler.OwnedStocks.FirstOrDefault(s => s.Stock.Name == stockName)!.Amount += amountDifference; 
            }
            else{
                srcDealler.OwnedStocks.FirstOrDefault(s => s.Stock.Name == stockName)!.Amount -= amountDifference;
                StockWithAmount stockToAdd=new StockWithAmount(srcDealler.OwnedStocks.FirstOrDefault(s => s.Stock.Name == stockName)!.Stock,amountDifference);
                dstDealler.OwnedStocks.Add(stockToAdd);
            }
        }
        public void removeOffer(Offer toRemove)
        {
            if (toRemove.Type == OfferType.buyingOffer)
                OffersB.Remove(toRemove.Owner.Name + " - " + toRemove.Stock.Name, out toRemove);
            else
                OffersS.Remove(toRemove.Owner.Name + " - " + toRemove.Stock.Name, out toRemove);
        }
        public Boolean isValidDeal(Dealler dealler, Offer offerToCheck)
        {
            if (offerToCheck.Type == OfferType.sellingOffer &&
            offerToCheck.Owner.OwnedStocks.FirstOrDefault(s => s.Stock.Name == offerToCheck.Stock.Name,null)!=null)
            {
                if (dealler.CurrMoney > offerToCheck.WantedPrice)
                    return true;
                return false;
            }
            if (offerToCheck.Type == OfferType.buyingOffer)
                return (dealler.OwnedStocks.FirstOrDefault(s=>s.Stock.Name==offerToCheck.Stock.Name,null)!=null);
            return false;
        }
        public Boolean isValidOffer(Offer toCheck)
        {
            return !this.OffersB.ContainsKey(toCheck.Owner.Name + " - " + toCheck.Stock.Name) &&
                    !this.OffersS.ContainsKey(toCheck.Owner.Name + " - " + toCheck.Stock.Name);
        }
        public void setStockCurrentPriceByName(string stockName, double price)
        {
            getStockByName(stockName).CurrentPrice = price;
        }
        public void updateHistory(Offer offerToAdd, Dealler dealler, Boolean deallerIsBuyer)
        {
            Deal deal;
            if (deallerIsBuyer)
                deal = new Deal(offerToAdd.Owner, dealler, offerToAdd.WantedPrice, offerToAdd.Amount);
            else
                deal = new Deal(dealler, offerToAdd.Owner, offerToAdd.WantedPrice, offerToAdd.Amount);

            DeallersHistory[dealler.Name].Enqueue(deal);
            DeallersHistory[offerToAdd.Owner.Name].Enqueue(deal);

            StocksHistory[offerToAdd.Stock.Name].Enqueue(deal);
        } 
        
        //general
        public void addStockToBuyerOwnedStocks(Stock stockToAdd, Dealler dealler, int amount)
        {
            StockWithAmount isExist=dealler.OwnedStocks.FirstOrDefault(d=>d.Stock.Id==stockToAdd.Id,null)!;
            if(isExist!=null) isExist.Amount+=amount;
            else dealler.OwnedStocks.Add(new StockWithAmount(stockToAdd, amount));
        }
        public Boolean insertOffer(Offer toAdd)
        {
            if (!isValidOffer(toAdd))
                return false;
            if (toAdd.Type == OfferType.buyingOffer)
                OffersB.TryAdd(toAdd.Owner.Name + " - " + toAdd.Stock.Name, toAdd);
            else
                OffersS.TryAdd(toAdd.Owner.Name + " - " + toAdd.Stock.Name, toAdd);
            return true;
        }
        public Offer getOfferByName(string offerName)
        {
            Offer res;
            if (OffersB.TryGetValue(offerName, out res!) || OffersS.TryGetValue(offerName, out res!))
                return res;
            return null!;
        }
        public Dealler getDeallerByName(string deallerName)
        {
            return this.Deallers.FirstOrDefault(d => d.Name == deallerName)!;
        }
        public Stock getStockByName(string stockName)
        {
            return this.Stocks.FirstOrDefault(d => d.Name == stockName)!;
        }
    }
}
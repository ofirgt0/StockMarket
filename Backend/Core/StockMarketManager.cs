namespace Backend.Core
{
    public class StockMarketManager : IStockMarketManager
    {

        const int TRADING_DAY = 10000;
        public static int counter = 0;

        public async Task activeBurse(IStockMarket context)
        {
            Console.Write(counter + " "); counter++;
            Random percentageRand = new Random();
            _ = changeStockPriceEvent(context, percentageRand);
            _ = updateOffers(context);
            _ = Task.Run(async () => await Task.Delay(TRADING_DAY)).ContinueWith(async (_) => await activeBurse(context));
        }

        public async Task changeStockPriceEvent(IStockMarket context, Random percentageRand)
        {
            double percentageDifference;
            foreach (var stock in context.Stocks)
            {
                percentageDifference = Math.Round(percentageRand.NextDouble() * (5.0 - (-5.0)) - 5, 2);
                stock.CurrentPrice += Math.Round(((double)stock.CurrentPrice! * percentageDifference) / 100);
                stock.PercentageDifference.Push(percentageDifference);
            }
            await Task.Delay(TRADING_DAY);
        }
        public async Task updateOffers(IStockMarket context)
        {
            double latestStockValueUpdate;
            foreach (KeyValuePair<string, Offer> offer in context.OffersB)
            {
                latestStockValueUpdate = offer.Value.Stock.PercentageDifference.Pop();
                offer.Value.WantedPrice += (latestStockValueUpdate * offer.Value.WantedPrice) / 100;
            }
            foreach (KeyValuePair<string, Offer> offer in context.OffersS)
            {
                latestStockValueUpdate = offer.Value.Stock.PercentageDifference.Pop();
                offer.Value.WantedPrice += (latestStockValueUpdate * offer.Value.WantedPrice) / 100;
            }
        }
        
    }
}
namespace Backend.Core
{
    public class StockMarketManager : IStockMarketManager
    {
        private readonly IConfiguration Configuration;
        const int TRADING_DAY = 10000;
        public static int counter = 0;

        public StockMarketManager(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public async Task activeBurse(IStockMarket context)
        {
            Console.Write(counter + " "); counter++;
            Random percentageRand = new Random();
            await changeStockPrice(context, percentageRand);
            await updateOffers(context);
            _ = Task.Run(async () => await Task.Delay(TRADING_DAY)).ContinueWith(async (_) => await activeBurse(context));
        }

        public async Task changeStockPrice(IStockMarket context, Random percentageRand)
        {
            double percentageDifference;
            foreach (var stock in context.Stocks)
            {
                percentageDifference = Math.Round(percentageRand.NextDouble() * (5.0 - (-5.0)) - 5, 2);
                stock.CurrentPrice += Math.Round(((double)stock.CurrentPrice! * percentageDifference) / 100);
                stock.PercentageDifference.Push(percentageDifference);
            }
            await Task.Delay(int.Parse(Configuration["CONSTANT:TRADING_DAY"]));
        }
        public async Task updateOffers(IStockMarket context)
        {
            double latestStockValueUpdate;
            foreach (KeyValuePair<string, Offer> offer in context.BuyingOffer)
            {
                latestStockValueUpdate = offer.Value.Stock.PercentageDifference.Peek();
                offer.Value.WantedPrice += (latestStockValueUpdate * offer.Value.WantedPrice) / 100;
            }
            foreach (KeyValuePair<string, Offer> offer in context.SellingOffer)
            {
                latestStockValueUpdate = offer.Value.Stock.PercentageDifference.Peek();
                offer.Value.WantedPrice += (latestStockValueUpdate * offer.Value.WantedPrice) / 100;
            }
        }
    }
}
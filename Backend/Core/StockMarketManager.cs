using static Backend.Core.ConfigurationManager;

namespace Backend.Core
{
    public class StockMarketManager : IStockMarketManager
    {
        private readonly IConfiguration Configuration;
        public static int counter = 0;

        public StockMarketManager(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public async Task activeBurse(IStockMarket context)
        {
            Console.WriteLine("Trading Day - " + counter); counter++;
            Random percentageRand = new Random();
            await changeStockPrice(context, percentageRand);
            await updateOffers(context);
            _ = Task.Run(async () => await Task.Delay(int.Parse(Configuration[CONSTANT + ":" + TRADING_DAY]))).ContinueWith(async (_) => await activeBurse(context));
        }

        public async Task changeStockPrice(IStockMarket context, Random percentageRand)
        {
            double percentageDifference;
            foreach (var stock in context.Stocks)
            {
                percentageDifference = Math.Round(percentageRand.NextDouble() * (5.0 - (-5.0)) - 5, 2);
                stock.CurrentPrice += (double)stock.CurrentPrice! * percentageDifference / 100;
                stock.PercentageDifference.Push(percentageDifference);
            }
            await Task.Delay(int.Parse(Configuration[CONSTANT + ":" + TRADING_DAY]));
        }

        public async Task updateOffers(IStockMarket context)
        {
            context.UpdateOffers();
        }
    }
}
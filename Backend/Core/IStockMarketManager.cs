namespace Backend.Core
{
    public interface IStockMarketManager
    {
        public Task activeBurse(IStockMarket context);
        public Task changeStockPrice(IStockMarket context, Random rand);
    }
}
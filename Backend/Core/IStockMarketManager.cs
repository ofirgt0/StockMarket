namespace Backend.Core
{
    public interface IStockMarketManager
    {
        public Task activeBurse(IStockMarket context);
        public Task changeStockPriceEvent(IStockMarket context, Random rand);

    }
}
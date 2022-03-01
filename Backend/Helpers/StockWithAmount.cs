namespace Backend.Helpers
{
    public class StockWithAmount
    {
        public StockWithAmount(Stock ownedStock, int amount)
        {
            Stock = new Stock(ownedStock);
            _amount = amount;
        }

        public Stock Stock { get; set; }
        public int Amount { get { return _amount; } }
        private int _amount;

        public void decreaseAmountSafely(int amount)
        {
            Interlocked.Increment(ref _amount);
            _amount -= amount;
        }
        public void increaseAmountSafely(int amount)
        {
            Interlocked.Increment(ref _amount);
            _amount += amount;
        }
    }
}
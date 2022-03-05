namespace Backend.Helpers
{
    public class StockWithAmount
    {
        public StockWithAmount(Stock ownedStock, int amount)
        {
            _amountLocker=new object();
            Stock = new Stock(ownedStock);
            _amount = amount;
        }

        public Stock Stock { get; set; }
        public int Amount { get { return _amount; } }
        private int _amount;
        private readonly object _amountLocker;
        public void decreaseAmountSafely(int amount)
        {
            lock(_amountLocker){
                _amount -= amount;
            }
            
        }
        public void increaseAmountSafely(int amount)
        {
             lock(_amountLocker){
                _amount += amount;
            }
        }
    }
}
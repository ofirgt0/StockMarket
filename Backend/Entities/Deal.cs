namespace Backend.Entities
{
    public class Deal
    {
        public Deal(string stockName, string seller, string buyer, double price, int amount)
        {
            Seller = seller;
            Buyer = buyer;
            Price = price;
            Amount = amount;
            StockName=stockName;
            DealTime = DateTime.Now;
        }

        public string Seller { get; set; }
        public string Buyer { get; set; }
        public double Price { get; set; }
        public int Amount { get; set; }
        public string StockName{ get; set; }
        public DateTime DealTime { get; set; }
    }
}
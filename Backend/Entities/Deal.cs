namespace Backend.Entities
{
    public class Deal
    {
        public Deal(Dealler seller, Dealler buyer, double price, int amount)
        {
            Seller = seller;
            Buyer = buyer;
            Price = price;
            Amount = amount;
            DealTime = DateTime.Now;
        }

        public Dealler Seller { get; set; }
        public Dealler Buyer { get; set; }
        public double Price { get; set; }
        public int Amount { get; set; }
        public DateTime DealTime { get; set; }
    }
}
namespace Backend.Helpers
{
    public class OfferTypeClass
    {
        public enum OfferType
        {
            buyingOffer,
            sellingOffer
        }
    }
    public class StockWithAmount
    {
        
        public StockWithAmount(Stock ownedStock, int amount)
        {
            Stock = new Stock(ownedStock);
            Amount = amount;
        }

        public Stock Stock { get; set; }
        public int Amount {get; set;}
    }

}
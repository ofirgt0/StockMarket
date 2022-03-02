
using Backend.Helpers;

namespace Backend
{
    public class Offer
    {

        public Dealler Owner { get; set; }

        public Stock Stock { get; set; }

        public double WantedPrice { get; set; }

        public int OfferStockAmount { get; set; }

        public OfferType Type { get; set; }

        public Offer(Dealler owner, Stock stock, double wantedPrice, OfferType type, int amount)
        {
            Owner = owner;
            Stock = stock;
            WantedPrice = wantedPrice;
            Type = type;
            OfferStockAmount = amount;
        }

        public Offer(Offer toCopy, int newAmount)
        {
            Owner = toCopy.Owner;
            Stock = toCopy.Stock;
            WantedPrice = toCopy.WantedPrice;
            OfferStockAmount = newAmount;
            Type = toCopy.Type;
        }
    }
}
using static Backend.Entities.OfferTypeEnum;

namespace Backend
{
    public class Offer
    {
        
        public Dealler Owner { get; set; }

        public Stock Stock { get; set; }

        public double WantedPrice { get; set; }

        public int Amount{get; set;}

        public OfferType Type { get; set; }

        public Offer(Dealler owner, Stock stock, double wantedPrice, OfferType type,int amount)
        {
            Owner = owner;
            Stock = stock;
            WantedPrice = wantedPrice;
            Type = type;
            Amount=amount;
        }

    }
}
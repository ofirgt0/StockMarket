namespace Backend.Helpers
{
    public class OfferTypeClass
    {
        public static OfferType getType(string type)
        {
            switch (type)
            {
                case "buy": return OfferType.buyingOffer;
                case "sell": return OfferType.sellingOffer;
                default: return OfferType.none;
            }
        }
    }
}
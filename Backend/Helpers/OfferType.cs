namespace Backend.Helpers
{
    public static class OfferTypeClass
    {
        public enum OfferType
        {
            buyingOffer,
            sellingOffer,
            none
        }
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
namespace Backend.Entities
{
    public class HoldingsWorth
    {
        public double TotalWorth { get; set; }
        public Dictionary<string,double> OwnedStockWorth { get; set; }
        public double MoneyAtOpening { get; set; }
        public double CurrMoney { get; set; }
    
        public HoldingsWorth(double totalWorth, double moneyAtOpening, double currMoney)
        {
            TotalWorth = totalWorth;
            OwnedStockWorth = new Dictionary<string, double>();
            MoneyAtOpening = moneyAtOpening;
            CurrMoney = currMoney;
        }
    }
}
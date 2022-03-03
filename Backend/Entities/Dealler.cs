using Backend.Helpers;
using Newtonsoft.Json;

namespace Backend
{
    public class Dealler
    {
        public Dealler(int id, string name, int moneyAtOpening)
        {
            Id = id;
            Name = name;
            MoneyAtOpening = moneyAtOpening;
            CurrMoney = moneyAtOpening;
            OwnedStocks = new List<StockWithAmount>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        [JsonProperty("money")]
        public double MoneyAtOpening { get; set; }
        public double CurrMoney { get; set; }
        public List<StockWithAmount> OwnedStocks { get; set; }
        public int OwnedStocksAmount{ get; set; }

        public string toString()
        {
            return "id: " + Id + "\nname" + Name + "\nmoneyAtOpening" + "\ncurrent money" + CurrMoney;
        }
    }
}
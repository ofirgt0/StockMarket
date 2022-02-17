using Backend.Entities;

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
            ownedStocks=new List<Stock>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public double MoneyAtOpening { get; set; }
        public double CurrMoney { get; set; }
        public List<Stock> ownedStocks { get; set; }

        public string toString()
        {
            return "id: " + Id + "\nname" + Name + "\nmoneyAtOpening" + "\ncurrent money" + CurrMoney;
        }
    }
}
using Backend.Entities;

namespace Backend
{
    public class Stock
    {
        public int Id {get; set;}
        public string Name {get;set;}
        public double CurrentPrice{get;set;}
        public int Amount{get;set;}
        public int CurrentStockAmountInBurse{get;set;}
        public Stack<double> PercentageDifference { get; set; }
        public Stock(int id, string name, double currentPrice, int amount)
        {
            Id = id;
            Name = name;
            CurrentPrice = currentPrice;
            Amount = amount;
            CurrentStockAmountInBurse=amount;
            PercentageDifference=new Stack<double>();
        }

        
    }
}
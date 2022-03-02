namespace Backend.Entities
{
    public class RequestBodyParams
    {
        public string deallerName{get;set;} 
        public string stockName{get;set;}
        public double wantedPrice{get;set;}
        public int wantedAmount{get;set;}
        public string type{get;set;}
    }
}
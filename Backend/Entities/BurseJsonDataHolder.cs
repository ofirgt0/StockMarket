namespace Backend.Entities
{
    public class BurseJsonDataHolder
    {
        public BurseJsonDataHolder(List<Dealler> traders, List<Stock> shares)
        {
            this.traders = traders;
            this.shares = shares;
        }

        public List<Dealler> traders { get; set; }
        public List<Stock> shares { get; set; }
    }
}
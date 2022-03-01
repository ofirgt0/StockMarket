namespace Backend.Core
{
    public class ConfigurationManager
    {
        public const string Constant = "CONSTANT";

        public string TradingDay { get; set; } = String.Empty;
        public string JsonPath { get; set; } = String.Empty;
    }
}
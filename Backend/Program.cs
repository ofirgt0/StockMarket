using Backend.Core;

namespace API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;

            var IStockMarketService = services.GetRequiredService<IStockMarket>();
            var IStockMarketManagerService = services.GetRequiredService<IStockMarketManager>();

            await IStockMarketManagerService.activeBurse(IStockMarketService);
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}

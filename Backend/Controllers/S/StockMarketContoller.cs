using Microsoft.AspNetCore.Mvc;
using Backend.Core;
using System.Collections.Concurrent;
using Backend.Helpers;
using static Backend.Helpers.OfferTypeClass;

namespace Backend.Controllers
{

    // [Route("api/[controller]")]
    // [ApiController]
    public class StockMarketController : ControllerBase
    {
        IStockMarket _stockMarketData;

        public StockMarketController(IStockMarket stockMarketData)
        {
            _stockMarketData = stockMarketData;
        }

        [HttpGet("stocks")]
        public List<Stock> GetStocks()
        {
            return _stockMarketData.Stocks;
        }

        [HttpGet("stocks/{name}")]
        public async Task<ActionResult<Stock>> GetStockAsync(string name)
        {
            return _stockMarketData.GetStockByName(name);
        }

        [HttpGet("stock/{id}")]
        public async Task<ActionResult<Stock>> GetStockByIdAsync(int id)
        {
            return _stockMarketData.GetStockById(id);
        }

        [HttpGet("deallers")]
        public async Task<ActionResult<List<Dealler>>> GetDeallersAsync()
        {
            return _stockMarketData.Deallers;
        }

        [HttpGet("deallers/{name}")]
        public async Task<ActionResult<Dealler>> GetDeallerAsync(string name)
        {
            return _stockMarketData.GetDeallerByName(name);
        }

        [HttpGet("dealler/{id}")]
        public async Task<ActionResult<Dealler>> GetDeallerById(int id)
        {
            return _stockMarketData.GetDeallerById(id);
        }

        [HttpGet("offers/buying")]
        public async Task<ActionResult<ConcurrentDictionary<string, Offer>>> GetBuyingOffersAsync()
        {
            return _stockMarketData.BuyingOffer;
        }

        [HttpGet("offers/selling")]
        public async Task<ActionResult<ConcurrentDictionary<string, Offer>>> GetSellingOffersAsync()
        {
            return _stockMarketData.SellingOffer;
        }

        [HttpPost("offers/newOffer")]
        public async Task<ActionResult<Offer>> NewOfferAsync(string deallerName, string stockName, double wantedPrice, int amount, string type)
        {
            Dealler activeDealler = _stockMarketData.GetDeallerByName(deallerName);
            Stock SellingOffertock = _stockMarketData.GetStockByName(stockName);
            OfferType offerType = (type == "buyingOffer") ? OfferType.buyingOffer : OfferType.sellingOffer;
            Offer newOffer = new Offer(activeDealler, SellingOffertock, wantedPrice, offerType, amount);

            return (_stockMarketData.InsertOffer(newOffer)) ? Ok(newOffer) : NotFound(404);
        }

        [HttpPost("offers/makeADeal")]
        public async Task<ActionResult<MakeADealResponse>> MakeADealAsync(string deallerName, string stockName, double wantedPrice, int wantedAmount, string type)
        {
            return _stockMarketData.MakeADeal(deallerName, stockName, wantedPrice, wantedAmount, OfferTypeClass.getType(type));
        }

    }
}
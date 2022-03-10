using Microsoft.AspNetCore.Mvc;
using Backend.Core;
using System.Collections.Concurrent;
using Backend.Helpers;
using Backend.Entities;

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
            return _stockMarketData.BuyingOffers;
        }

        [HttpGet("offers/selling")]
        public async Task<ActionResult<ConcurrentDictionary<string, Offer>>> GetSellingOffersAsync()
        {
            return _stockMarketData.SellingOffers;
        }

        [HttpPost("offers/newOffer")]
        public async Task<ActionResult<bool>> NewOfferAsync([FromBody] RequestBodyParams bodyParams)
        {
            return (_stockMarketData.InsertOffer(bodyParams.deallerName, bodyParams.stockName, bodyParams.wantedPrice, bodyParams.wantedAmount, bodyParams.type));
        }

        [HttpPost("offers/makeADeal")]
        public async Task<ActionResult<MakeADealResponse>> MakeADealAsync([FromBody] RequestBodyParams bodyParams)
        {

            MakeADealResponse ret = _stockMarketData.MakeADeal(bodyParams.deallerName, bodyParams.stockName, bodyParams.wantedPrice, bodyParams.wantedAmount, OfferTypeClass.getType(bodyParams.type));
            Console.WriteLine("ret - " + ret.Action);
            return ret;
        }
        [HttpGet("deals/{name}")]
        public async Task<ActionResult<List<Deal>>> GetDeallerDealsAsync(string name)
        {
            List<Deal> ret = _stockMarketData.GetDeallerDeals(name);
            
            return ret;
        }
        [HttpGet("stocks/worth/{name}")]
        public async Task<ActionResult<HoldingsWorth>> GetDeallerHoldingsWorthAsync(string name)
        {
            HoldingsWorth ret = _stockMarketData.GetDeallersWorth(name);
            return ret;
        }

    }
}
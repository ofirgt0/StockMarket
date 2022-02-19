using Microsoft.AspNetCore.Mvc;
using Backend.Core;
using System.Collections.Concurrent;
using Backend.Helpers;
using static Backend.Helpers.OfferTypeClass;

namespace Backend.Controllers
{

    //[Route("api/[controller]")]
    //[ApiController]
    public class StockMarketController : ControllerBase
    {
        IStockMarket _burseData;

        public StockMarketController(IStockMarket burseData)
        {
            _burseData = burseData;
        }

        [HttpGet("stocks")]
        public List<Stock> GetStocks()
        {
            return _burseData.Stocks;
        }
        [HttpGet("stocks/{name}")]
        public async Task<ActionResult<Stock>> GetStock(string name)
        {
            return _burseData.getStockByName(name);
        }
        [HttpGet("deallers")]
        public async Task<ActionResult<List<Dealler>>> GetDeallers()
        {
            return _burseData.Deallers;
        }
        [HttpGet("deallers/{name}")]
        public async Task<ActionResult<Dealler>> GetDealler(string name)
        {
            return _burseData.getDeallerByName(name);
        }
        [HttpGet("offers/buying")]
        public async Task<ActionResult<ConcurrentDictionary<string, Offer>>> GetBuyingOffers()
        {
            return _burseData.OffersB;
        }
        [HttpGet("offers/selling")]
        public async Task<ActionResult<ConcurrentDictionary<string, Offer>>> GetSellingOffers()
        {
            return _burseData.OffersS;
        }
        [HttpPost("offers/newOffer/{deallerName}/{stockName}/{wantedPrice}/{amount}/{type}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Offer>> NewOffer(string deallerName, string stockName, double wantedPrice, int amount, string type)
        {
            Dealler activeDealler = _burseData.getDeallerByName(deallerName);
            Stock offersStock = _burseData.getStockByName(stockName);
            OfferType offerType = (type == "buyingOffer") ? OfferType.buyingOffer : OfferType.sellingOffer;
            Offer newOffer = new Offer(activeDealler, offersStock, wantedPrice, offerType, amount);

            return (_burseData.insertOffer(newOffer)) ? Ok(newOffer) : NotFound(new APIResponse(404));
        }
        [HttpPost("offers/makeADeal/{offerName}/{deallerName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Offer>> MakeADeal(string offerName, string deallerName)
        {
            return (_burseData.makeADeal(offerName,deallerName)) ? Ok(offerName) : NotFound(new APIResponse(404));
        }

    }
}
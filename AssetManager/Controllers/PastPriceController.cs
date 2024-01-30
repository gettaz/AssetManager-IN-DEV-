using AssetManager.DTO;
using AssetManager.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AssetManager.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PastPriceController : ControllerBase
    {
        private readonly IPriceService _priceService;

        public PastPriceController(IPriceService priceService)
        {
            _priceService = priceService;
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<TimelineSummaryDto>> GetPastPrices(string userId)
        {
            try
            {
                var timelineSummary = await _priceService.GetHistoricalCategoryPriceAsync(userId);
                if (timelineSummary == null || timelineSummary.Prices.Count() == 0)
                {
                    return NotFound($"No past prices found for symbol {userId}.");
                }

                return Ok(timelineSummary);
            }
            catch (System.Exception ex)
            {
                // Log the exception
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}

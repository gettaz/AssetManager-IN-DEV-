﻿using AssetManager.ActionFilers.Filters;
using AssetManager.DTO;
using AssetManager.Helper;
using AssetManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static AssetManager.Helper.UserIdExtractor;
namespace AssetManager.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    [ServiceFilter(typeof(ExceptionFilter))]
    public class PastPriceController : ControllerBase
    {
        private readonly IPriceService _priceService;

        public PastPriceController(IPriceService priceService)
        {
            _priceService = priceService;
        }

        [HttpGet("historic")]
        public async Task<ActionResult<TimelineSummaryDto>> GetPastPrices()
        {
            
            try
            {
                var timelineSummary = await _priceService.GetHistoricalCategoryPriceAsync(UserIdExtract());
                if (timelineSummary == null || timelineSummary.Prices.Count() == 0)
                {
                    return NotFound($"No past prices found for user {UserIdExtract()}.");
                }

                return Ok(timelineSummary);
            }
            catch (System.Exception ex)
            {
                // Log the exception
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        private string UserIdExtract()
        {
            return HttpContext.Items["UserId"] as string;
        }
    }
}

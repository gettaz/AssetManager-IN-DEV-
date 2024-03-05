using AssetManager.ActionFilers.Filters;
using AssetManager.DTO;
using AssetManager.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static AssetManager.Helper.UserIdExtractor;

namespace AssetManager.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ServiceFilter(typeof(ExceptionFilter))]

    public class AssetClassificationsController : Controller
    {
        private readonly ILogger<AssetClassificationsController> _logger;
        private readonly IAssetClassificationsService _classificationsService;

        public AssetClassificationsController(ILogger<AssetClassificationsController> logger, IAssetClassificationsService classificationsService)
        {
            _logger = logger;
            _classificationsService = classificationsService;
        }

        [HttpGet("{classificationType}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<string>))]
        public IActionResult GetClassifications([FromQuery] string classificationType)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                IEnumerable<ClassificationDto> classifications = classificationType switch
                {
                    "broker" => _classificationsService.GetBrokers(UserIdExtract()),
                    "category" => _classificationsService.GetCategories(UserIdExtract()),
                    _ => throw new ArgumentException("Invalid classification type")
                };

                if (classifications == null || !classifications.Any())
                    return NotFound();

                return Ok(classifications.Select(c => c.Name).ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred in GetClassifications: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet("distribution")]
        public IActionResult GetClassificationDistribution([FromQuery] string classificationType)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                IEnumerable<ClassificationAssetCount> classifications = classificationType switch
                {
                    "broker" => _classificationsService.GetBrokersAssetCount(UserIdExtract()),
                    "category" => _classificationsService.GetCategoriesAssetCount(UserIdExtract()),
                    _ => throw new ArgumentException("Invalid classification type")
                };

                if (classifications == null || !classifications.Any())
                    return NotFound();

                return Ok(classifications);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred in GetClassificationDistribution: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        private string UserIdExtract()
        {
            return HttpContext.Items["UserId"] as string;
        }
    }
}

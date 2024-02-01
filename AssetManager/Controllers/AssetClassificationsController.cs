using AssetManager.DTO;
using AssetManager.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static AssetManager.Helper.UserIdExtractor;

namespace AssetManager.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class AssetClassificationsController : Controller
    {
        private readonly ILogger<AssetClassificationsController> _logger;
        private readonly IAssetClassificationsService _classificationsService;

        public AssetClassificationsController(ILogger<AssetClassificationsController> logger, IAssetClassificationsService classificationsService)
        {
            _logger = logger;
            _classificationsService = classificationsService;
        }

        [HttpGet("classification")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<string>))] // TODO shouldnt have logic move the twitchcase to middleware
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

        // Endpoint for creating a classification
        [HttpPost("create")]
        public IActionResult CreateClassification([FromQuery] string classificationType, [FromBody] ClassificationDto classification)
        {
            if (classification == null || !ModelState.IsValid)
                return BadRequest(ModelState);

            bool success = classificationType switch
            {
                "broker" => _classificationsService.CreateBroker(UserIdExtract(), classification),
                "category" => _classificationsService.CreateCategory(UserIdExtract(), classification),
                _ => throw new ArgumentException("Invalid classification type")
            };

            if (!success)
            {
                ModelState.AddModelError("", "Something went wrong while saving or classification already exists");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created");
        }

        // Endpoint for updating a classification
        [HttpPost("update")]
        public IActionResult UpdateClassification(string classificationType, [FromBody] ClassificationDto classification)
        {
            if (classification == null || !ModelState.IsValid)
                return BadRequest(ModelState);

            bool success = classificationType switch
            {
                "broker" => _classificationsService.UpdateBroker(UserIdExtract(), classification),
                "category" => _classificationsService.UpdateCategory(UserIdExtract(), classification),
                _ => throw new ArgumentException("Invalid classification type")
            };

            if (!success)
            {
                ModelState.AddModelError("", "Something went wrong while updating or classification with such name already exists");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully updated");
        }

        // Endpoint for deleting a classification
        [HttpDelete("delete")]
        public IActionResult RemoveClassification(int classificationId, [FromQuery] string classificationType)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            bool success = classificationType switch
            {
                "broker" => _classificationsService.DeleteBroker(UserIdExtract(), classificationId),
                "category" => _classificationsService.DeleteCategory(UserIdExtract(), classificationId),
                _ => throw new ArgumentException("Invalid classification type")
            };

            if (!success)
            {
                ModelState.AddModelError("", "Something went wrong while saving");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully removed");
        }
        private string UserIdExtract()
        {
            return HttpContext.Items["UserId"] as string;
        }
    }
}

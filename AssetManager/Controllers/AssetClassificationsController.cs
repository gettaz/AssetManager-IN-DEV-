using AssetManager.DTO;
using AssetManager.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AssetManager.Controllers
{
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

        // Combined endpoint for getting either categories or brokers
        [HttpGet("{userId}/classifications")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<string>))]
        public IActionResult GetClassifications(string userId, string classificationType)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            IEnumerable<ClassificationDto> classifications = classificationType switch
            {
                "broker" => _classificationsService.GetBrokers(userId),
                "category" => _classificationsService.GetCategories(userId),
                _ => throw new ArgumentException("Invalid classification type")
            };

            if (classifications.IsNullOrEmpty())
                return NotFound();

            return Ok(classifications.Select(c=>c.Name).ToList());
        }

        [HttpGet("{userId}")]
        public IActionResult GetClassificationDistribution(string userId, string classificationType)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            IEnumerable<ClassificationAssetCount> classifications = classificationType switch
            {
                "broker" => _classificationsService.GetBrokersAssetCount(userId),
                "category" => _classificationsService.GetCategoriesAssetCount(userId),
                _ => throw new ArgumentException("Invalid classification type")
            };

            if (classifications.IsNullOrEmpty())
                return NotFound();

            return Ok(classifications);
        }

        // Endpoint for creating a classification
        [HttpPost("{userId}")]
        public IActionResult CreateClassification(string userId, string classificationType, [FromBody] ClassificationDto classification)
        {
            if (classification == null || !ModelState.IsValid)
                return BadRequest(ModelState);

            bool success = classificationType switch
            {
                "broker" => _classificationsService.CreateBroker(userId, classification),
                "category" => _classificationsService.CreateCategory(userId, classification),
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
        [HttpPost("{userId}/update")]
        public IActionResult UpdateClassification(string userId, string classificationType, [FromBody] ClassificationDto classification)
        {
            if (classification == null || !ModelState.IsValid)
                return BadRequest(ModelState);

            bool success = classificationType switch
            {
                "broker" => _classificationsService.UpdateBroker(userId, classification),
                "category" => _classificationsService.UpdateCategory(userId, classification),
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
        [HttpDelete("{userId}/{classificationId}")]
        public IActionResult RemoveClassification(string userId, int classificationId, string classificationType)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            bool success = classificationType switch
            {
                "broker" => _classificationsService.DeleteBroker(userId, classificationId),
                "category" => _classificationsService.DeleteCategory(userId, classificationId),
                _ => throw new ArgumentException("Invalid classification type")
            };

            if (!success)
            {
                ModelState.AddModelError("", "Something went wrong while saving");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully removed");
        }
    }
}

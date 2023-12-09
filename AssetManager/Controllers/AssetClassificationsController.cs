using AssetManager.DTO;
using AssetManager.Interfaces;
using AssetManager.Models;
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

        [HttpGet("{userId}/categories")]
        public IActionResult GetCategories(string userId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var categories = _classificationsService.GetCategories(userId);

            if (categories.IsNullOrEmpty())
            {
                return NotFound();
            }

            return Ok(categories);
        }

        [HttpGet("{userId}/brokers")]
        public IActionResult GetBrokers(string userId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var brokers = _classificationsService.GetBrokers(userId);

            if (brokers.IsNullOrEmpty())
            {
                return NotFound();
            }

            return Ok(brokers);
        }


        [HttpPost("{userId}/categories")]
        public IActionResult CreateCategory(string userId, [FromBody] ClassificationDto category)
        {
            if (category == null || !ModelState.IsValid)
                return BadRequest(ModelState);

            bool success = _classificationsService.CreateCategory(userId, category);
            if (!success)
            {
                ModelState.AddModelError("", "Something went wrong while saving or category already exists");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created");
        }

        [HttpPost("{userId}/categories/update")]
        public IActionResult UpdateCategory(string userId, [FromBody] ClassificationDto category)
        {
            if (category == null || !ModelState.IsValid)
                return BadRequest(ModelState);

            bool success = _classificationsService.UpdateCategory(userId, category);
            if (!success)
            {
                ModelState.AddModelError("", "Something went wrong while updating or category with such name already exists");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully updated");
        }

        [HttpDelete("{userId}/categories/{categoryId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult RemoveCategory(string userId, int categoryId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_classificationsService.DeleteCategory(userId, categoryId))
            {
                ModelState.AddModelError("", "Something went wrong while saving");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully removed");
        }
    }
}

using AssetManager.DTO;
using AssetManager.Interfaces;
using AssetManager.Models;
using AssetManager.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AssetManager.Controllers
{
    [Route("api/[controller]")]

    public class CategoryController : Controller
    {
        private ICategoryRepository _categoryRepository;
        private readonly ILogger<CategoryController> _logger;
        private readonly IMapper _mapper;

        public CategoryController(ILogger<CategoryController> logger,ICategoryRepository categoryRepository, IMapper mapper)
        {
            _logger = logger;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        [HttpGet("{userId}/categories")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CategoryDto>))]

        public IActionResult GetCategories(string userId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var categories = _categoryRepository.GetUserCategories(userId);

            if (categories.IsNullOrEmpty())
            {
                return NotFound(ModelState);
            }

            return Ok(categories);
        }


        [HttpPost("categories")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateCategory([FromBody] CategoryDto category)
        {
            if (category == null)
                return BadRequest(ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var categoryMap = _mapper.Map<Category>(category);
            var existingCategory = _categoryRepository.GetUserCategories(categoryMap.UserId);
            if (existingCategory.Any(c => c.Name == category.Name))
            {
                ModelState.AddModelError("", "There is already a category with such name");
                return StatusCode(409, ModelState);
            }

            if (!_categoryRepository.CreateCategory(categoryMap))
            {
                ModelState.AddModelError("", "Something went wrong while saving");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created");
        }

        [HttpPost("categories/update")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult UpdateCategory([FromBody] CategoryDto category)
        {
            if (category == null)
                return BadRequest(ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var categories = _categoryRepository.GetUserCategories(category.UserId);
            if (categories.Any(c => c.Name == category.Name))
            {
                ModelState.AddModelError("", "There is already a category with such name");
                return StatusCode(409, ModelState);
            }

            var current = categories.FirstOrDefault(c => c.Id == category.Id);
            if (current == null)
            {
                return NotFound(ModelState);
            }

            current.Name = category.Name;
            var categoryMap = _mapper.Map<Category>(current);
            if (!_categoryRepository.UpdateCategory(categoryMap))
            {
                ModelState.AddModelError("", "Something went wrong while updating");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created");
        }

        [HttpDelete("{userId}/categories/{categoryId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult RemoveCategory(string userId, int categoryId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_categoryRepository.DeleteCategory(userId, categoryId))
            {
                ModelState.AddModelError("", "Something went wrong while saving");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully removed");
        }
    }
}

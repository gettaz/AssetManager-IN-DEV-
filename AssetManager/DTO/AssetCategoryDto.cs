using System.ComponentModel.DataAnnotations;

namespace AssetManager.DTO
{
    namespace AssetManager.DTO
    {
        public class AssetCategoryDto
        {
            [Required(ErrorMessage = "AssetId is required")]
            public int AssetId { get; set; }

            [Required(ErrorMessage = "CategoryId is required")]
            public int CategoryId { get; set; }
        }
    }

}

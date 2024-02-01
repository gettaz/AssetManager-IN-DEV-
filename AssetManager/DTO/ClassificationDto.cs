using System.ComponentModel.DataAnnotations;

namespace AssetManager.DTO
{
    public class ClassificationDto
    {
        [Required(ErrorMessage = "id is required")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
    }
}

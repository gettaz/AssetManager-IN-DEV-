using System.ComponentModel.DataAnnotations;

namespace AssetManager.DTO
{
    public class RegisterDto
    {
        public string Username { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }
    }
}

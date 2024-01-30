using AssetManager.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AssetManager.Controllers
{
    [Route("api/[controller]")]

    public class AccountController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        [HttpPost("register")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var user = new IdentityUser { UserName = dto.Username };
            var result = await _userManager.CreateAsync(user, dto.Password);
            
            if (result.Succeeded)
            {
                user.EmailConfirmed = true; // Add this line to confirm the email
                await _userManager.UpdateAsync(user); // And this line to save the change
                return Ok();
            }

            // If we got this far, something failed.
            return BadRequest(result.Errors);
        }
        [HttpPost("login")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _userManager.FindByNameAsync(dto.Username);
            if (user == null)
            {
                // Handle case when there is no user with the provided username
                return Unauthorized("Invalid username.");
            }

            var result = await _signInManager.PasswordSignInAsync(user, dto.Password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var tokenOptions = _configuration.GetSection("TokenOptions").Get<TokenOptions>();
                var key = Encoding.UTF8.GetBytes(tokenOptions.Secret);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id)
                     }),
                    Issuer = tokenOptions.Issuer,
                    Audience = tokenOptions.Audience,
                    Expires = DateTime.UtcNow.AddDays(tokenOptions.ExpiryDays),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new { token = tokenHandler.WriteToken(token) });
            }

            if (result.IsNotAllowed)
            {
                return Unauthorized("User is not allowed.");
            }

            // Handle other cases such as when the user is locked out
            return Unauthorized("Invalid login attempt.");
        }
    }

}

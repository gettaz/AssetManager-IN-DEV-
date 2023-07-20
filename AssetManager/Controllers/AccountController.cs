using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AssetManager.Controllers
{
    [Route("api/[controller]")]

    public class AccountController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost("register")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Register(string username, string password)
        {
            var user = new IdentityUser { UserName = username };
            var result = await _userManager.CreateAsync(user, password);
            
            if (result.Succeeded)
            {
                user.EmailConfirmed = true; // Add this line to confirm the email
                await _userManager.UpdateAsync(user); // And this line to save the change
                await _signInManager.SignInAsync(user, isPersistent: false);
                return Ok();
            }

            // If we got this far, something failed.
            return BadRequest(result.Errors);
        }
        [HttpPost("login")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                // Handle case when there is no user with the provided username
                return Unauthorized("Invalid username.");
            }

            var result = await _signInManager.PasswordSignInAsync(user, password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return Ok(new { UserId = user.Id });  // Return the user's Id in the response
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

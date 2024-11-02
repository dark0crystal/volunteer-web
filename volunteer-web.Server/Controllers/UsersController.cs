using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using volunteer_web.Server.Models;
using System.Collections.Generic;
using System.Linq;

namespace volunteer_web.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        public static readonly List<UserModel> Users = new List<UserModel>();

        // Allow new users to register
        [HttpPost("register")]
        public ActionResult<UserModel> Register([FromBody] UserModel user)
        {
            if (user == null || string.IsNullOrEmpty(user.firstName) || string.IsNullOrEmpty(user.email) || string.IsNullOrEmpty(user.password))
            {
                return BadRequest("Invalid user data.");
            }

            if (Users.Any(u => u.email.Equals(user.email, StringComparison.OrdinalIgnoreCase)))
            {
                return Conflict("User with this email already exists.");
            }

            user.id = Users.Count > 0 ? Users.Max(u => u.id) + 1 : 1;
            Users.Add(user);
            return CreatedAtAction(nameof(GetAllUsers), new { id = user.id }, user);
        }

        // Login method
        [HttpPost("login")]
        public ActionResult<UserModel> Login([FromBody] LoginModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
            {
                return BadRequest("Invalid email or password.");
            }

            var existingUser = Users.FirstOrDefault(u => u.email == model.Email && u.password == model.Password);

            if (existingUser == null)
            {
                return Unauthorized("Invalid email or password.");
            }

            // Set cookie for the logged-in user
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true, // Prevents JavaScript access
                Expires = DateTimeOffset.UtcNow.AddDays(7) // Cookie expiration
            };
            Response.Cookies.Append("UserEmail", model.Email, cookieOptions);

            return Ok(existingUser);
        }

        // Logout method
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("UserEmail");
            return Ok("Logged out successfully.");
        }

        // Return all users
        [HttpGet("all")]
        public ActionResult<IEnumerable<UserModel>> GetAllUsers()
        {
            return Ok(Users);
        }
    }
}


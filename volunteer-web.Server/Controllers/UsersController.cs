using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using volunteer_web.Server.Data;

namespace volunteer_web.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Integrated Security=True;Trust Server Certificate=True";

        // Allow new users to register
        [HttpPost("register")]
        public IActionResult Register([FromBody] UserModel user)
        {
            MasterContext context = new MasterContext();

            if (user == null || string.IsNullOrEmpty(user.firstName) || string.IsNullOrEmpty(user.email) || string.IsNullOrEmpty(user.password))
            {
                return BadRequest("Invalid user data.");
            }

            try
            {
              


                //using Lambda Expression to check if the user is already in the database
                var isUserAlreadyRegistered = context.Registers.Any(s => s.Email == user.email);

                if (isUserAlreadyRegistered) {
                    return BadRequest("user already registered");
                }

                // SQL query to insert a new user
                string query = @"INSERT INTO register (firstName, familyName, email, password)
                                 VALUES (@firstName, @familyName, @email, @password)";


                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@firstName", user.firstName);
                        cmd.Parameters.AddWithValue("@familyName", user.familyName);
                        cmd.Parameters.AddWithValue("@email", user.email);
                        cmd.Parameters.AddWithValue("@password", user.password);

                        cmd.ExecuteNonQuery();
                    }
                }

                return Ok(new { message = "User registered successfully." });
            }
            catch (SqlException ex)
            {
                return StatusCode(500, new { message = "A database error occurred.", error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", error = ex.Message });
            }
        }

        // Check if the user is logged in by validating session or cookie
        [HttpGet("check-session")]
        public IActionResult CheckSession()
        {
            var userEmail = HttpContext.Session.GetString("UserEmail"); // Check the session for UserEmail

            if (string.IsNullOrEmpty(userEmail))
            {
                return Unauthorized(new { message = "User is not logged in." }); // Return Unauthorized if the session is null
            }

            return Ok(new { message = "User is logged in.", userEmail });
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
            {
                return BadRequest("Invalid email or password.");
            }

            try
            {
               MasterContext context = new MasterContext(); 
                {
                    // Use LINQ to validate user login
                    var existingUser = context.Registers
                        .Where(u => u.Email == model.Email && u.Password == model.Password)
                        .Select(u => new UserModel
                        {
                            id = u.Id,
                            firstName = u.FirstName,
                            familyName = u.FamilyName,
                            email = u.Email
                        })
                        .FirstOrDefault();

                    if (existingUser == null)
                    {
                        return Unauthorized("Invalid email or password.");
                    }

                    // Set session data
                    HttpContext.Session.SetInt32("UserId", existingUser.id);
                    HttpContext.Session.SetString("UserEmail", existingUser.email);
                    HttpContext.Session.SetString("UserName", existingUser.firstName);

                    // Set cookie for the logged-in user
                    var cookieOptions = new CookieOptions
                    {
                        HttpOnly = true, // Prevents JavaScript access
                        Expires = DateTimeOffset.UtcNow.AddDays(7) // Cookie expiration
                    };
                    Response.Cookies.Append("UserEmail", model.Email, cookieOptions);

                    return Ok(existingUser);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", error = ex.Message });
            }
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // Clear session data
            HttpContext.Session.Remove("UserEmail");
            HttpContext.Session.Remove("UserName");
            HttpContext.Session.Remove("UserId");

            // Clear cookies
            Response.Cookies.Delete("UserEmail");

            return Ok(new { message = "User logged out successfully" });
        }


        // Return all users
        [HttpGet("all")]
        public IActionResult GetAllUsers()
        {
            try
            {
                var users = new List<UserModel>();

                // SQL query to fetch all users
                string query = "SELECT id, firstName, familyName, email FROM register";

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                users.Add(new UserModel
                                {
                                    id = reader.GetInt32(0),
                                    firstName = reader.GetString(1),
                                    familyName = reader.GetString(2),
                                    email = reader.GetString(3)
                                });
                            }
                        }
                    }
                }

                return Ok(users);
            }
            catch (SqlException ex)
            {
                return StatusCode(500, new { message = "A database error occurred.", error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", error = ex.Message });
            }
        }
    }

    // User model
    public class UserModel
    {
        public int id { get; set; }
        public string firstName { get; set; }
        public string familyName { get; set; }
        public string email { get; set; }
        public string password { get; set; }
    }

    // Login model
    public class LoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}

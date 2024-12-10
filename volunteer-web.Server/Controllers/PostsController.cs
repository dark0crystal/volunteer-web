using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using volunteer_web.Server.Data;
using System.Xml;

namespace volunteer_web.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PostsController : ControllerBase
    {
        string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Integrated Security=True;Trust Server Certificate=True";

        // Get all posts
        [HttpGet("posts")]
        public ActionResult<IEnumerable<Posts>> Get()
        {
            var posts = new List<Posts>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                var query = "SELECT * FROM VolunteeringPosts";
                var command = new SqlCommand(query, connection);
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        posts.Add(new Posts
                        {
                            id = reader.GetInt32(0),
                            title = reader.GetString(1),
                            category = (PostCategory)Enum.Parse(typeof(PostCategory), reader.GetString(2)),
                            description = reader.GetString(3),
                            numOfDays = reader.GetInt32(4),
                            location = reader.GetString(5),
                            orgName = reader.GetString(6),
                            startDate = reader.IsDBNull(7) ? null : reader.GetDateTime(7).ToString("yyyy-MM-dd"),
                            endDate = reader.IsDBNull(8) ? null : reader.GetDateTime(8).ToString("yyyy-MM-dd"),
                            postAdminEmail = reader.GetString(9)
                        });
                    }
                }
            }

            return Ok(posts);
        }





        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            try
            {
                using (MasterContext context = new MasterContext())
                {
                    // Fetch the post
                    var post = await context.VolunteeringPosts
                        .Include(p => p.Complaints) // Include related complaints
                        .FirstOrDefaultAsync(p => p.Id == id);

                    if (post == null)
                        return NotFound("Post not found.");

                    // Delete associated complaints
                    context.Complaints.RemoveRange(post.Complaints);

                    // Delete the post
                    context.VolunteeringPosts.Remove(post);

                    // Save changes
                    await context.SaveChangesAsync();
                }

                return Ok("Post and associated complaints deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the post.", error = ex.Message });
            }
        }

        [HttpGet("count-posts-by-category")]
        public async Task<IActionResult> GetPostsCountByCategory()
        {
            try
            {
                using (MasterContext context = new MasterContext())
                {
                    // Grouping posts by Category and counting them
                    var postCount = await context.VolunteeringPosts
                        .GroupBy(p => p.Category) // Grouping by category 
                        .Select(g => new
                        {
                            Category = g.Key,
                            PostCount = g.Count()
                        })
                        .ToListAsync();

                    return Ok(postCount);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching post counts.", error = ex.Message });
            }
        }



        // Get posts related to the logged-in user
        [HttpPost("dashboard")]
        public ActionResult<IEnumerable<Posts>> Dashboard()
        {
            var email = HttpContext.Session.GetString("UserEmail");

            if (string.IsNullOrEmpty(email))
            {
                
                return Unauthorized(new { message = "Please log in to access the dashboard." });
            }

            var posts = new List<Posts>();
            using (var connection = new SqlConnection(connectionString))
            {
                var query = "SELECT * FROM VolunteeringPosts WHERE PostAdminEmail = @PostAdminEmail";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@PostAdminEmail", email);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        posts.Add(new Posts
                        {
                            id = reader.GetInt32(0),
                            title = reader.GetString(1),
                            category = (PostCategory)Enum.Parse(typeof(PostCategory), reader.GetString(2)),
                            description = reader.GetString(3),
                            numOfDays = reader.GetInt32(4),
                            location = reader.GetString(5),
                            orgName = reader.GetString(6),
                            startDate = reader.IsDBNull(7) ? null : reader.GetDateTime(7).ToString("yyyy-MM-dd"),
                            endDate = reader.IsDBNull(8) ? null : reader.GetDateTime(8).ToString("yyyy-MM-dd"),
                            postAdminEmail = reader.GetString(9)
                        });
                    }
                }
            }

            return posts.Any() ? Ok(posts) : NotFound(new { message = "No posts found." });
        }

        [HttpGet("post-details/{postId}")]
        public ActionResult<Posts> GetPostDetails(int postId)
        {
            Posts post = null;
            using (var connection = new SqlConnection(connectionString))
            {
                var query = "SELECT * FROM VolunteeringPosts WHERE Id = @Id";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Id", postId);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        post = new Posts
                        {
                            id = reader.GetInt32(0),
                            title = reader.GetString(1),
                            category = (PostCategory)Enum.Parse(typeof(PostCategory), reader.GetString(2)),
                            description = reader.GetString(3),
                            numOfDays = reader.GetInt32(4),
                            location = reader.GetString(5),
                            orgName = reader.GetString(6),
                            startDate = reader.IsDBNull(7) ? null : reader.GetDateTime(7).ToString("yyyy-MM-dd"),
                            endDate = reader.IsDBNull(8) ? null : reader.GetDateTime(8).ToString("yyyy-MM-dd"),
                            postAdminEmail = reader.GetString(9)
                        };
                    }
                }
            }

            return post != null ? Ok(post) : NotFound("Post not found.");
        }

        // Update an existing post
        [HttpPut("updatepost/{id}")]
        public IActionResult UpdatePost(int id, [FromBody] UpdatePost updatePost)
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userEmail))
            {
                return Unauthorized(new { Message = "User not logged in" });
            }

            using (var connection = new SqlConnection(connectionString))
            {
                var query = @"UPDATE VolunteeringPosts
                      SET Title = @Title,
                          Category = @Category,
                          Description = @Description,
                         
                          Location = @Location,
                          OrgName = @OrgName,
                          StartDate = @StartDate,
                          EndDate = @EndDate
                      WHERE Id = @Id AND PostAdminEmail = @PostAdminEmail";

                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Id", id);
                command.Parameters.AddWithValue("@Title", updatePost.title);
                command.Parameters.AddWithValue("@Category", updatePost.category.ToString());
                command.Parameters.AddWithValue("@Description", updatePost.description);
                command.Parameters.AddWithValue("@NumOfDays", updatePost.numOfDays);
                command.Parameters.AddWithValue("@Location", updatePost.location);
                command.Parameters.AddWithValue("@OrgName", updatePost.orgName);
                command.Parameters.AddWithValue("@StartDate", updatePost.startDate ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@EndDate", updatePost.endDate ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@PostAdminEmail", userEmail);

                connection.Open();
                var result = command.ExecuteNonQuery();

                return result > 0 ? Ok(new { Message = "Post updated successfully" }) : BadRequest("Failed to update post.");
            }
        }
        [HttpPost("save-post")]
        public IActionResult SavePost([FromBody] int postId)
        {
            var email = HttpContext.Session.GetString("UserEmail");

            if (string.IsNullOrEmpty(email) || postId <= 0)
            {
                return BadRequest("Invalid request data.");
            }

            try
            {
                using (var connection = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Integrated Security=True;Trust Server Certificate=True"))
                {
                    connection.Open();

                    // Fetch the user ID based on the email
                    var userIdQuery = "SELECT Id FROM Register WHERE Email = @Email";
                    var userIdCommand = new SqlCommand(userIdQuery, connection);
                    userIdCommand.Parameters.AddWithValue("@Email", email);
                    var userId = userIdCommand.ExecuteScalar();

                    if (userId == null)
                    {
                        return Unauthorized("User not found.");
                    }

                    // Check if the post exists
                    var postExistsQuery = "SELECT COUNT(1) FROM VolunteeringPosts WHERE Id = @PostId";
                    var postExistsCommand = new SqlCommand(postExistsQuery, connection);
                    postExistsCommand.Parameters.AddWithValue("@PostId", postId);
                    var postExists = (int)postExistsCommand.ExecuteScalar() > 0;

                    if (!postExists)
                    {
                        return NotFound("Post not found.");
                    }

                    // Check if the post is already saved by the user
                    var savedPostExistsQuery = "SELECT COUNT(1) FROM SavedPosts WHERE UserId = @UserId AND PostId = @PostId";
                    var savedPostExistsCommand = new SqlCommand(savedPostExistsQuery, connection);
                    savedPostExistsCommand.Parameters.AddWithValue("@UserId", userId);
                    savedPostExistsCommand.Parameters.AddWithValue("@PostId", postId);
                    var savedPostExists = (int)savedPostExistsCommand.ExecuteScalar() > 0;

                    if (savedPostExists)
                    {
                        return BadRequest("You have already saved this post.");
                    }

                    // Insert the new saved post record
                    var insertQuery = "INSERT INTO SavedPosts (UserId, PostId) VALUES (@UserId, @PostId)";
                    var insertCommand = new SqlCommand(insertQuery, connection);
                    insertCommand.Parameters.AddWithValue("@UserId", userId);
                    insertCommand.Parameters.AddWithValue("@PostId", postId);
                    insertCommand.ExecuteNonQuery();

                    return Ok("You have successfully saved the post.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred: " + ex.Message);
            }
        }


        [HttpGet("user-volunteering")]
        public IActionResult GetUserVolunteeringPosts()
        {
            var email = HttpContext.Session.GetString("UserEmail");

            if (string.IsNullOrEmpty(email))
            {
                return Unauthorized("User is not logged in.");
            }

            try
            {
                using (var connection = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Integrated Security=True;Trust Server Certificate=True"))
                {
                    connection.Open();

                    // Fetch the user ID based on the email
                    var userIdQuery = "SELECT Id FROM Register WHERE Email = @Email";
                    var userIdCommand = new SqlCommand(userIdQuery, connection);
                    userIdCommand.Parameters.AddWithValue("@Email", email);
                    var userId = userIdCommand.ExecuteScalar();

                    if (userId == null)
                    {
                        return Unauthorized("User not found.");
                    }

                    // Get the user's saved posts
                    var savedPostsQuery = @"
                SELECT p.Id, p.Title, p.Category, p.Description, p.NumOfDays, p.Location, p.OrgName, p.StartDate, p.EndDate, p.PostAdminEmail
                FROM VolunteeringPosts p
                INNER JOIN SavedPosts sp ON p.Id = sp.PostId
                WHERE sp.UserId = @UserId";

                    var savedPostsCommand = new SqlCommand(savedPostsQuery, connection);
                    savedPostsCommand.Parameters.AddWithValue("@UserId", userId);

                    var reader = savedPostsCommand.ExecuteReader();

                    var posts = new List<VolunteeringPost>();
                    while (reader.Read())
                    {
                        posts.Add(new VolunteeringPost
                        {
                            Id = reader.GetInt32(0),
                            Title = reader.GetString(1),
                            Category = reader.GetString(2),
                            Description = reader.GetString(3),
                            NumOfDays = reader.GetInt32(4),
                            Location = reader.GetString(5),
                            OrgName = reader.GetString(6),
                            StartDate = reader.IsDBNull(7) ? null : DateOnly.FromDateTime(reader.GetDateTime(7)),
                            EndDate = reader.IsDBNull(8) ? null : DateOnly.FromDateTime(reader.GetDateTime(8)),
                            PostAdminEmail = reader.GetString(9)
                        });
                    }

                    return Ok(posts);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred: " + ex.Message);
            }
        }



        // Add a new post
        [HttpPost("addpost")]
        public IActionResult AddPost([FromBody] AddPost AddPost)
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userEmail))
            {
                return Unauthorized(new { Message = "User not logged in" });
            }

            using (var connection = new SqlConnection(connectionString))
            {
                var query = @"INSERT INTO VolunteeringPosts (Title, Category, Description, NumOfDays, Location, OrgName, StartDate, EndDate, PostAdminEmail)
                              VALUES (@Title, @Category, @Description, @NumOfDays, @Location, @OrgName, @StartDate, @EndDate, @PostAdminEmail)";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Title", AddPost.title);
                command.Parameters.AddWithValue("@Category", AddPost.category.ToString());
                command.Parameters.AddWithValue("@Description", AddPost.description);
                command.Parameters.AddWithValue("@NumOfDays", AddPost.numOfDays);
                command.Parameters.AddWithValue("@Location", AddPost.location);
                command.Parameters.AddWithValue("@OrgName", AddPost.orgName);
                command.Parameters.AddWithValue("@StartDate", AddPost.startDate ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@EndDate", AddPost.endDate ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@PostAdminEmail", userEmail);

                connection.Open();
                var result = command.ExecuteNonQuery();

                return result > 0 ? Ok(new { Message = "Post created successfully" }) : BadRequest("Failed to create post.");
            }
        }
    }
    public class UpdatePost
    {
        public string title { get; set; }
        public int category { get; set; }
        public string description { get; set; }
        public int numOfDays { get; set; }
        public string location { get; set; }
        public string orgName { get; set; }
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
    }

    public class Posts
    {
        public int id { get; set; }
        public string postAdminEmail { get; set; }
        public string title { get; set; }
        public PostCategory category { get; set; }
        public string description { get; set; }
        public int numOfDays { get; set; }
        public string location { get; set; }
        public string orgName { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }



    }
    public enum PostCategory
    {
        Education,
        Environment,
        Health,
        Community,
        AnimalWelfare,

    }
    public class AddPost
    {

        public string title { get; set; }
        public PostCategory category { get; set; }
        public string description { get; set; }
        public int numOfDays { get; set; }
        public string location { get; set; }
        public string orgName { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }

    }

}

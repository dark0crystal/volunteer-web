using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using volunteer_web.Server.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Hosting;

namespace volunteer_web.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PostsController : ControllerBase
    {
        // Sample list of posts
        public static List<PostsModel> Posts = new List<PostsModel>
        {
            new PostsModel { id = 1, title = "Tech Childrens", category = PostCategory.Education, description = "Description 1", numOfDays = 5, location = "Muscat, Alkhodh 7", orgName = "Org 1", startDate = "2024-01-01", endDate = "2024-01-05", postAdminEmail="user@gmail.com"},
            new PostsModel { id = 2, title = "Planting Trees", category = PostCategory.Environment, description = "Description 2", numOfDays = 3, location = "Al Sharqiya North , Samad Alshaan", orgName = "Org 2", startDate = "2024-02-01", endDate = "2024-02-04", postAdminEmail="user@gmail.com" },
            new PostsModel { id = 3, title = "Nursing Assistant", category = PostCategory.Health, description = "Description 3", numOfDays = 7, location = "Al Sharqiya North ,Ibra", orgName = "Org 3", startDate = "2024-03-01", endDate = "2024-03-08", postAdminEmail="user3@gmail.com"},
            new PostsModel { id = 4, title = "Distributing Donations", category = PostCategory.Community, description = "Description 4", numOfDays = 10, location = "Dhofar , Salalah", orgName = "Org 4", startDate = "2024-04-01", endDate = "2024-04-11", postAdminEmail="user4@gmail.com" },
        };

        // Get all posts
        [HttpGet("posts")]
        public ActionResult<IEnumerable<PostsModel>> Get()
        {
            return Ok(Posts);
        }

        // Get posts related to the logged-in user
        [HttpPost("dashboard")]
        public ActionResult<IEnumerable<PostsModel>> Dashboard()
        {
            // Retrieve the email from session
            var email = HttpContext.Session.GetString("UserEmail");
            Console.WriteLine("this the email  :", email);

            if (string.IsNullOrEmpty(email))
            {
                return Unauthorized("Please log in to access the dashboard.");
            }

            var similarPosts = Posts.Where(p => p.postAdminEmail == email).ToList();

            if (!similarPosts.Any())
            {
                return NotFound("No similar posts found.");
            }

            return Ok(similarPosts);
        }


        [HttpGet("post-details/{postId}")]
        public ActionResult<PostsModel> GetPostDetails(int postId)
        {
            var post = Posts.FirstOrDefault(p => p.id == postId);
            if (post == null)
            {
                return NotFound("Post not found.");
            }
            return Ok(post);
        }

        // Add a new post
        [HttpPost("addpost")]
        public IActionResult AddPost([FromBody] AddPostModel addPostModel)
        {
            // Retrieve the email from session
            var userEmail = HttpContext.Session.GetString("UserEmail");
            Console.WriteLine("this the email  :", userEmail);
            Console.WriteLine(userEmail);
            if (string.IsNullOrEmpty(userEmail))
            {
                return Unauthorized(new { Message = "User not logged in" });
            }

            // Create the post model including the user's email
            var post = new PostsModel
            {
                // Generate a new unique ID for the post (using max ID + 1 or a GUID)
                id = Posts.Any() ? Posts.Max(p => p.id) + 1 : 1,
                postAdminEmail = userEmail,
                title = addPostModel.title,
                category = addPostModel.category,
                description = addPostModel.description,
                numOfDays = addPostModel.numOfDays,
                location = addPostModel.location,
                orgName = addPostModel.orgName,
                startDate = addPostModel.startDate,
                endDate = addPostModel.endDate
            };

            // Add the new post to the static list
            Posts.Add(post);

            return Ok(new { Message = "Post created successfully", Post = post });
        }




        public static List<SavePostRequestModel> UserPostRelations = new List<SavePostRequestModel>();

        [HttpPost("save-post")]
        public ActionResult SavePost([FromBody] int PostId)
        {
            var email = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(email) || PostId <= 0)
            {
                return BadRequest("Invalid request data.");
            }

            var post = Posts.FirstOrDefault(p => p.id ==PostId);
            if (post == null)
            {
                return NotFound("Post not found.");
            }

            UserPostRelations.Add(new SavePostRequestModel
            {
                email = email,
                postId = PostId
            });

            return Ok("You're successfully added");
        }

        [HttpGet("user-volunteering")]
        public ActionResult<List<PostsModel>> GetUserVolunteeringPosts()
        {
            var email = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(email))
            {
                return Unauthorized("User is not logged in.");
            }

            // Get PostIds associated with the current user's email
            var userPostIds = UserPostRelations
                .Where(rel => rel.email == email)
                .Select(rel => rel.postId)
                .ToList();

            // Find the matching posts
            var userPosts = Posts.Where(post => userPostIds.Contains(post.id)).ToList();

            return Ok(userPosts);
        }



    }
}

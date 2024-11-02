using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using volunteer_web.Server.Models;
using System.Collections.Generic;
using System.Linq;

namespace volunteer_web.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PostsController : ControllerBase
    {
        // Sample list of posts
        public static List<PostsModel> Posts = new List<PostsModel>
        {
            new PostsModel { id = 1, title = "Mohd", category = PostCategory.Education, description = "Description 1", numOfDays = 5, location = "Location 1", orgName = "Org 1", startDate = "2024-01-01", endDate = "2024-01-05", postAdminEmail="user@gmail.com"},
            new PostsModel { id = 2, title = "Ahmed", category = PostCategory.Environment, description = "Description 2", numOfDays = 3, location = "Location 2", orgName = "Org 2", startDate = "2024-02-01", endDate = "2024-02-04", postAdminEmail="user@gmail.com" },
            new PostsModel { id = 3, title = "Said", category = PostCategory.Health, description = "Description 3", numOfDays = 7, location = "Location 3", orgName = "Org 3", startDate = "2024-03-01", endDate = "2024-03-08", postAdminEmail="user3@gmail.com"},
            new PostsModel { id = 4, title = "Saif", category = PostCategory.Community, description = "Description 4", numOfDays = 10, location = "Location 4", orgName = "Org 4", startDate = "2024-04-01", endDate = "2024-04-11", postAdminEmail="user4@gmail.com" },
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
            // Read the email from the cookie
            var email = Request.Cookies["UserEmail"];

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

        // Get post details by ID
        [HttpPost("post-details")]
        public ActionResult<PostsModel> PostDetails([FromBody] int postId)
        {
            if (postId <= 0)
            {
                return BadRequest("Invalid post ID.");
            }

            var postDetails = Posts.FirstOrDefault(p => p.id == postId);
            if (postDetails == null)
            {
                return NotFound("No details found.");
            }
            return Ok(postDetails);
        }

        // Add a new post
        [HttpPost("addpost")]
        public ActionResult<PostsModel> AddPost([FromBody] AddPostModel post)
        {
            if (post == null || string.IsNullOrEmpty(post.title) || string.IsNullOrEmpty(post.description) || string.IsNullOrEmpty(post.orgName))
            {
                return BadRequest("Invalid post data.");
            }

            int newId = Posts.Max(p => p.id) + 1;
            var newPost = new PostsModel
            {
                id = newId,
                title = post.title,
                category = post.category,
                description = post.description,
                numOfDays = post.numOfDays,
                location = post.location,
                orgName = post.orgName,
                startDate = post.startDate,
                endDate = post.endDate,
                postAdminEmail = Request.Cookies["UserEmail"] // Get email from cookie
            };

            Posts.Add(newPost);
            return CreatedAtAction(nameof(Get), new { id = newPost.id }, newPost);
        }


        public static List<SavePostRequestModel> UserPostRelations = new List<SavePostRequestModel>();
        [HttpPost("save-post")]
        public ActionResult SavePost([FromBody] SavePostRequestModel request)
        {
            if (request == null || string.IsNullOrEmpty(request.email) || request.PostId <= 0)
            {
                return BadRequest("Invalid request data.");
            }

            var post = Posts.FirstOrDefault(p => p.id == request.PostId);
            if (post == null)
            {
                return NotFound("Post not found.");
            }

            // Store the relation in the list
            UserPostRelations.Add(new SavePostRequestModel
            {
                email = request.email,
                PostId = request.PostId
            });

            return Ok("Post relation saved successfully.");
        }
    }
}

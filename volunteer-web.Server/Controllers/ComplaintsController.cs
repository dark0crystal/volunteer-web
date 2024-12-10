using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using volunteer_web.Server.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;


namespace volunteer_web.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ComplaintsController : ControllerBase
    {
        private readonly MasterContext _context;

        public ComplaintsController(MasterContext context)
        {
            _context = context;
        }

        [HttpPost("SubmitComplaint")]
        public IActionResult SubmitComplaint([FromBody] ComplaintRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.ComplaintText))
            {
                return BadRequest(new { message = "Complaint text is required." });
            }

            try
            {

                // Define the connection string
                string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Integrated Security=True;Trust Server Certificate=True";

                // SQL query to insert a complaint
                string query = @"INSERT INTO Complaints (PostId, ComplaintType, ComplaintText)
                                 VALUES (@PostId, @ComplaintType, @ComplaintText)";

                // Create and open the connection
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    // Create and configure the command
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        // Add parameters to prevent SQL injection
                        cmd.Parameters.AddWithValue("@PostId", request.PostId);
                        cmd.Parameters.AddWithValue("@ComplaintType", request.ComplaintType);
                        cmd.Parameters.AddWithValue("@ComplaintText", request.ComplaintText);

                        // Execute the command
                        cmd.ExecuteNonQuery();
                    }
                }

                return Ok(new { message = "Your complaint has been submitted successfully." });
            }
            catch (SqlException ex)
            {
                // Handle SQL errors
                return StatusCode(500, new { message = "A database error occurred.", error = ex.Message });
            }
            catch (Exception ex)
            {
                // Handle general errors
                return StatusCode(500, new { message = "An unexpected error occurred.", error = ex.Message });
            }
        }



        // Get the number of complaints grouped by post
        [HttpGet("grouped-by-post")]
        public async Task<IActionResult> GetComplaintsGroupedByPost()
        {
            try
            {
                // LINQ query to group complaints by Post Title and count them
                var complaintCounts = await _context.Complaints
                    .GroupBy(c => c.Post.Title) // Group by Post Title
                    .Select(g => new
                    {
                        PostTitle = g.Key,         // Post Title
                        ComplaintCount = g.Count() // Number of complaints for this post
                    })
                    .ToListAsync();

                if (!complaintCounts.Any())
                {
                    return NotFound("No complaints found.");
                }

                return Ok(complaintCounts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "An error occurred while fetching complaints.",
                    Error = ex.Message
                });
            }
        }
        public class ComplaintRequest
        {
            public int PostId { get; set; }
            public string ComplaintType { get; set; }
            public string ComplaintText { get; set; }
        }
        private readonly string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Integrated Security=True;Trust Server Certificate=True";

        // GET: Complaints/GetComplaintsByPost
        [HttpGet("GetComplaintsByPost")]
        public IActionResult GetComplaintsByPost([FromQuery] int postId)
        {
            try
            {
                // Validate the input
                

                if (postId <= 0)
                {
                    return BadRequest(new { message = "Invalid Post ID provided." });
                }

                Console.WriteLine($"Post ID: {postId}");

                MasterContext context = new MasterContext();

                // Use LINQ query syntax to fetch complaints
                var complaints = (from s in context.Complaints
                                  where s.PostId == postId
                                  select s).ToList();

                //// Check if no complaints were found
                if (complaints.Count == 0)
                {
                    return Ok(new List<ComplaintResponse>()); // Return an empty JSON array
                }

                // Log the complaints for debugging purposes
                foreach (var complaint in complaints)
                    {
                        Console.WriteLine(complaint.ToString());
                    }

                    return Ok(complaints);
                
            }
            catch (Exception ex)
            {
                // Handle any unexpected exceptions
                return StatusCode(500, new
                {
                    message = "An error occurred while retrieving complaints.",
                    error = ex.Message
                });
            }
        }

        // ComplaintResponse class
        public class ComplaintResponse
        {
            public int Id { get; set; }
            public int PostId { get; set; }
            public string ComplaintType { get; set; }
            public string ComplaintText { get; set; }

            public override string ToString()
            {
                return $"Id: {Id}, PostId: {PostId}, ComplaintType: {ComplaintType}, ComplaintText: {ComplaintText}";
            }

        }
    }
}

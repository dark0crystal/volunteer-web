using Microsoft.AspNetCore.Mvc;

namespace volunteer_web.Server.Controllers
{
    [ApiController]
    [Route("api/complaints")]
    public class ComplaintsController : ControllerBase
    {
        [HttpPost("SubmitComplaint")]
        public IActionResult SubmitComplaint([FromForm] ComplaintRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.ComplaintText))
            {
                return BadRequest(new { message = "Complaint text is required." });
            }


            return Ok(new { message = "Your complaint has been submitted successfully." });
        }
    }

    public class ComplaintRequest
    {
        public int PostId { get; set; }
        public string ComplaintType { get; set; }
        public string ComplaintText { get; set; }
    }
}

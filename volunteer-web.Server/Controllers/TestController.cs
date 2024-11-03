using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using volunteer_web.Server.Models;
using System.Collections.Generic;

namespace volunteer_web.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<PostsModel>> Test()
        {
            HttpContext.Response.Cookies.Append("Username", "Ahmed", new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddMinutes(30),
                HttpOnly = true,
                IsEssential = true
            });

            return Ok("Cookie has been set successfully."); // Return an appropriate response
        }
    }
}


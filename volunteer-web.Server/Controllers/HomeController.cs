using Microsoft.AspNetCore.Mvc;

namespace volunteer_web.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : Controller
    {
        // GET: /Home
        [HttpGet("index")]
        public IActionResult Index()
        {
            return View();
        }
    }
}

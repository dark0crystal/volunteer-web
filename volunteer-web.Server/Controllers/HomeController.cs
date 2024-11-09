using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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

        // GET: /Home/AboutPage
        [HttpGet("AboutPage")]
        public IActionResult GetAboutPage()
        {
            return View("~/Views/Index.cshtml");
        }

        
        [HttpGet("ComplaintsPage")]
        public IActionResult GetComplaintsPage()
        {
            return View("~/Views/ComplaintsPage.cshtml");
        }





    }


}
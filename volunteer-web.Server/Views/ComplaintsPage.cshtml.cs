using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace volunteer_web.Server.Views
{
    public class ComplaintsPageModel : PageModel
    {
        [BindProperty]
        public int PostId { get; set; }

        [BindProperty]
        public string ComplaintText { get; set; }

        [BindProperty]
        public string ComplaintType { get; set; } // Property for complaint type (Spam, Inappropriate, etc.)

        public string ConfirmationMessage { get; set; } // Property for confirmation message
    }
}

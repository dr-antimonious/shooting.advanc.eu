using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ShootingWebsite.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public RedirectResult? OnGet()
        {
            return HttpContext.Request.Cookies.ContainsKey("userId")
                ? Redirect("/Interface")
                : null;
        }
    }
}
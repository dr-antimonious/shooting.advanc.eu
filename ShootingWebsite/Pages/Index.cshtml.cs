using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ShootingWebsite.Pages
{
    public class IndexModel : PageModel
    {
        public RedirectResult? OnGet()
        {
            return HttpContext.Request.Cookies.ContainsKey("userId")
                ? Redirect("/Interface")
                : null;
        }
    }
}
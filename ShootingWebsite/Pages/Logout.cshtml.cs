using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ShootingWebsite.Pages
{
    public class Logout : PageModel
    {
        public RedirectResult OnGet()
        {
            Response.Cookies.Append("userId", String.Empty);
            return Redirect("/Index");
        }
    }
}

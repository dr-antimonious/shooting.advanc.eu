using System.Net.Mail;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ShootingWebsite.Pages
{
    public class Register : PageModel
    {
        public void OnGet()
        {
        }

        public async Task<RedirectResult?> OnPostAsync()
        {
            FirestoreDb db = FirestoreDb.Create("shootingdiary-orwima");

            if (String.IsNullOrWhiteSpace(Request.Form["username"].ToString()))
            {
                TempData["AlertDanger"] = "Please enter a username";
                return null;
            }

            CollectionReference collection = db.Collection("users");
            IAsyncEnumerable<DocumentReference> documents = collection.ListDocumentsAsync();

            await foreach (DocumentReference document in documents)
            {
                DocumentSnapshot temp = await document.GetSnapshotAsync();
                temp.TryGetValue<String>("username", out String user);
                if (user == Request.Form["username"].ToString())
                {
                    TempData["AlertDanger"] = "Username is already in use";
                    return null;
                }
            }

            TempData["username"] = Request.Form["username"].ToString();

            if (String.IsNullOrWhiteSpace(Request.Form["email"].ToString()))
            {
                TempData["AlertDanger"] = "Please enter a valid e-mail address";
                return null;
            }

            try
            {
                var testEmail = new MailAddress(Request.Form["email"].ToString());
            }
            catch (Exception ex)
            {
                TempData["AlertDanger"] = "Please enter a valid e-mail address";
                return null;
            }

            TempData["email"] = Request.Form["email"].ToString();

            if (String.IsNullOrWhiteSpace(Request.Form["password"].ToString()))
            {
                TempData["AlertDanger"] = "Please enter a password";
                return null;
            }

            return null;
        }
    }
}

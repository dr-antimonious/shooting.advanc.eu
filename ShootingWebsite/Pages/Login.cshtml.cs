using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Cryptography;
using System.Text;

namespace ShootingWebsite.Pages
{
    public class Login : PageModel
    {
        public async Task<RedirectResult?> OnGet()
        {
            if (Request.Cookies.TryGetValue("userId", out String? userId))
            {
                FirestoreDb db = FirestoreDb.Create("shootingdiary-orwima");

                CollectionReference collection = db.Collection("users");
                IAsyncEnumerable<DocumentReference> documentRefs =
                    collection.ListDocumentsAsync();

                await foreach (DocumentReference document in documentRefs)
                {
                    DocumentSnapshot temp = await document.GetSnapshotAsync();
                    if (temp.Id == userId)
                        return Redirect("/Interface");
                }
            }

            return null;
        }

        public async Task<RedirectResult?> OnPost()
        {
            FirestoreDb db = FirestoreDb.Create("shootingdiary-orwima");

            CollectionReference collection = db.Collection("users");
            IAsyncEnumerable<DocumentReference> documentRefs =
                collection.ListDocumentsAsync();
            List<DocumentSnapshot> documents = new();

            await foreach (DocumentReference document in documentRefs)
            {
                DocumentSnapshot temp = await document.GetSnapshotAsync();
                documents.Add(temp);
            }

            if (String.IsNullOrWhiteSpace(Request.Form["username"].ToString()))
            {
                TempData["AlertDanger"] = "Please enter a username";
                return null;
            }

            DocumentSnapshot? userDocument = null;
            foreach (DocumentSnapshot document in documents)
            {
                document.TryGetValue("username", out String user);
                if (user == Request.Form["username"].ToString())
                    userDocument = document;
            }

            if (userDocument is null)
            {
                TempData["AlertDanger"] = "Username does not exist";
                return null;
            }

            TempData["username"] = Request.Form["username"].ToString();

            if (String.IsNullOrWhiteSpace(Request.Form["password"].ToString()))
            {
                TempData["AlertDanger"] = "Please enter a password";
                return null;
            }

            byte[] encryptedPassword = SHA512.HashData(
                Encoding.Default.GetBytes(Request.Form["password"].ToString()));

            String encrypted = Convert.ToHexString(encryptedPassword).ToLower();
            if (userDocument.TryGetValue("password", out String password))
            {
                if (password != encrypted)
                {
                    TempData["AlertDanger"] = "Incorrect password";
                    return null;
                }

                Response.Cookies.Append("userId", userDocument.Id);
                return Redirect("/Interface");
            }

            TempData["AlertDanger"] = "Unknown error. Please try later or contact site admins.";
            return null;
        }
    }
}

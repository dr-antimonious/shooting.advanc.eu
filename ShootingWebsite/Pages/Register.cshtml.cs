using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using Google.Cloud.Firestore;
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

            CollectionReference collection = db.Collection("users");
            IAsyncEnumerable<DocumentReference> documentRefs = 
                collection.ListDocumentsAsync();
            List<DocumentSnapshot> documents = new ();

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

            foreach (DocumentSnapshot document in documents)
            {
                document.TryGetValue("username", out String user);
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

            foreach (DocumentSnapshot document in documents)
            {
                document.TryGetValue("email", out String user);
                if (user == Request.Form["email"].ToString())
                {
                    TempData["AlertDanger"] = "E-mail address is already in use";
                    return null;
                }
            }

            TempData["email"] = Request.Form["email"].ToString();

            if (String.IsNullOrWhiteSpace(Request.Form["password"].ToString()))
            {
                TempData["AlertDanger"] = "Please enter a password";
                return null;
            }

            byte[] encryptedPassword = SHA512.HashData(
                Encoding.Default.GetBytes(Request.Form["password"].ToString()));

            String encrypted = Convert.ToHexString(encryptedPassword).ToLower();

            DocumentReference newUserRef = await collection.AddAsync(new
            {
                username = Request.Form["username"].ToString(),
                email = Request.Form["email"].ToString(),
                password = encrypted
            });

            DocumentSnapshot newUser = await newUserRef.GetSnapshotAsync();

            TempData["AlertSuccess"] = "User successfully created";
            Response.Cookies.Append("userId", newUser.Id);
            return Redirect("/Interface");
        }
    }
}

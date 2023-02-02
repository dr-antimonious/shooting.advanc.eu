using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ShootingWebsite.Pages
{
    public class IndexModel : PageModel
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
    }
}
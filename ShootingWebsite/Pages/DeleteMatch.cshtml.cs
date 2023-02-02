using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ShootingWebsite.Pages
{
    public class DeleteMatch : PageModel
    {
        public async Task<RedirectResult?> OnGet([FromQuery] String matchId)
        {
            bool valid = false;
            DocumentSnapshot? user = null;
            DocumentSnapshot? match = null;
            DocumentReference? matchRef = null;
            FirestoreDb db = FirestoreDb.Create("shootingdiary-orwima");

            if (String.IsNullOrWhiteSpace(matchId))
            {
                TempData["AlertDanger"] = "Something went wrong. Please try again later.";
                return Redirect("/Interface");
            }

            if (Request.Cookies.TryGetValue("userId", out String? userId))
            {
                if (String.IsNullOrWhiteSpace(userId))
                {
                    TempData["AlertDanger"] = "You are not logged in. Please log in or register.";
                    return Redirect("/Index");
                }

                CollectionReference collection = db.Collection("users");
                IAsyncEnumerable<DocumentReference> documentRefs =
                    collection.ListDocumentsAsync();

                await foreach (DocumentReference document in documentRefs)
                {
                    DocumentSnapshot temp = await document.GetSnapshotAsync();
                    if (temp.Id == userId)
                    {
                        user = temp;
                        valid = true;
                        break;
                    }
                }
            }

            else
            {
                TempData["AlertDanger"] = "You are not logged in. Please log in or register.";
                return Redirect("/Index");
            }

            if (!valid)
            {
                TempData["AlertDanger"] = "Incorrect userId found in cookies. Please login or register.";
                return Redirect("/Logout");
            }

            CollectionReference matchesRef = db.Collection("matches");
            IAsyncEnumerable<DocumentReference> matchRefs =
                matchesRef.ListDocumentsAsync();
            valid = false;

            await foreach (DocumentReference document in matchRefs)
            {
                DocumentSnapshot temp = await document.GetSnapshotAsync();
                if (temp.Id == matchId)
                {
                    if (temp.TryGetValue("userId", out String matchUser))
                    {
                        if (matchUser == userId)
                        {
                            matchRef = document;
                            match = temp;
                            valid = true;
                            break;
                        }
                    }
                }
            }

            if (!valid)
            {
                TempData["AlertDanger"] = "Requested match does not belong to this user.";
                return Redirect("/Interface");
            }

            TempData["matchId"] = matchId;
            CollectionReference seriesRef = matchRef.Collection("series");
            IAsyncEnumerable<DocumentReference> seriesRefs =
                seriesRef.ListDocumentsAsync();

            await foreach (DocumentReference document in seriesRefs)
            {
                await document.DeleteAsync();
            }

            await matchRef.DeleteAsync();
            TempData["AlertSuccess"] = "Match deleted";
            return Redirect("/Interface");
        }
    }
}

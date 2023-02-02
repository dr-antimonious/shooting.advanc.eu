using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ShootingWebsite.Pages
{
    public class MatchDetails : PageModel
    {
        public Dictionary<String, String> matchDictionary = new Dictionary<String, String>();
        public List<Dictionary<String, String>> series = new List<Dictionary<String, String>>();
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

            matchDictionary.Add("id", matchId);
            TempData["matchId"] = matchId;

            match.TryGetValue("AirPressure", out int airPressure);
            matchDictionary.Add("AirPressure", airPressure.ToString());

            match.TryGetValue("Date", out String date);
            matchDictionary.Add("Date", date);

            match.TryGetValue("EndTime", out String endTime);
            matchDictionary.Add("EndTime", endTime);

            match.TryGetValue("Humidity", out int humidity);
            matchDictionary.Add("Humidity", humidity.ToString());

            match.TryGetValue("Inner10s", out int inner10s);
            matchDictionary.Add("Inner10s", inner10s.ToString());

            match.TryGetValue("Location", out String location);
            matchDictionary.Add("Location", location);

            match.TryGetValue("Mood", out String mood);
            matchDictionary.Add("Mood", mood);

            match.TryGetValue("Notes", out String notes);
            matchDictionary.Add("Notes", notes);

            match.TryGetValue("Result", out int result);
            matchDictionary.Add("Result", result.ToString());

            match.TryGetValue("StartTime", out String startTime);
            matchDictionary.Add("StartTime", startTime);

            match.TryGetValue("Temperature", out int temperature);
            matchDictionary.Add("Temperature", temperature.ToString());

            CollectionReference seriesRef = matchRef.Collection("series");
            IAsyncEnumerable<DocumentReference> seriesRefs =
                seriesRef.ListDocumentsAsync();

            await foreach (DocumentReference document in seriesRefs)
            {
                DocumentSnapshot temp = await document.GetSnapshotAsync();
                temp.TryGetValue("Decimal", out Double dec);
                temp.TryGetValue("EndTime", out String seriesEndTime);
                temp.TryGetValue("Inner10s", out int seriesInner10s);
                temp.TryGetValue("Notes", out String seriesNotes);
                temp.TryGetValue("Result", out int seriesResult);
                temp.TryGetValue("StartTime", out String seriesStartTime);
                series.Add(new Dictionary<string, string>
                {
                    {"id", temp.Id},
                    {"Decimal", dec.ToString()},
                    {"Inner10s", seriesInner10s.ToString()},
                    {"Notes", seriesNotes},
                    {"Result", seriesResult.ToString()},
                    {"StartTime", seriesStartTime},
                    {"EndTime", seriesEndTime}
                });
            }

            return null;
        }
    }
}

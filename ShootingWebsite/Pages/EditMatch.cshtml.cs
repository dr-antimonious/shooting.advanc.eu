using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ShootingWebsite.Pages
{
    public class EditMatch : PageModel
    {
        public List<Dictionary<String, String>> series = new List<Dictionary<String, String>>();
        public async Task<RedirectResult?> OnGet([FromQuery] String matchId)
        {
            TempData.Clear();

            bool valid = false;
            DocumentSnapshot? user = null;
            DocumentSnapshot? match = null;
            DocumentReference? matchRef = null;
            FirestoreDb db = FirestoreDb.Create("shootingdiary-orwima");

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

            if (String.IsNullOrWhiteSpace(matchId))
                return null;

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

            match.TryGetValue("AirPressure", out int airPressure);
            TempData["airPressure"] = airPressure.ToString();

            match.TryGetValue("Date", out String date);
            TempData["date"] = date;

            match.TryGetValue("EndTime", out String endTime);
            TempData["endTime"] = endTime;

            match.TryGetValue("Humidity", out int humidity);
            TempData["humidity"] = humidity.ToString();

            match.TryGetValue("Inner10s", out int inner10s);
            TempData["inner10s"] = inner10s.ToString();

            match.TryGetValue("Location", out String location);
            TempData["location"] = location;

            match.TryGetValue("Mood", out String mood);
            TempData["mood"] = mood;

            match.TryGetValue("Notes", out String notes);
            TempData["notes"] = notes;

            match.TryGetValue("Result", out int result);
            TempData["result"] = result.ToString();

            match.TryGetValue("StartTime", out String startTime);
            TempData["startTime"] = startTime;

            match.TryGetValue("Temperature", out int temperature);
            TempData["temperature"] = temperature.ToString();

            CollectionReference seriesRef = matchRef.Collection("series");
            IAsyncEnumerable<DocumentReference> seriesRefs =
                seriesRef.ListDocumentsAsync();

            int i = 1;
            await foreach (DocumentReference document in seriesRefs)
            {
                DocumentSnapshot temp = await document.GetSnapshotAsync();
                temp.TryGetValue("Decimal", out Double dec);
                temp.TryGetValue("EndTime", out String seriesEndTime);
                temp.TryGetValue("Inner10s", out int seriesInner10s);
                temp.TryGetValue("Notes", out String seriesNotes);
                temp.TryGetValue("Result", out int seriesResult);
                temp.TryGetValue("StartTime", out String seriesStartTime);

                TempData[$"decimal-{i}"] = dec.ToString();
                TempData[$"endTime-{i}"] = seriesEndTime;
                TempData[$"inner10s-{i}"] = seriesInner10s.ToString();
                TempData[$"notes-{i}"] = seriesNotes;
                TempData[$"result-{i}"] = seriesResult.ToString();
                TempData[$"startTime-{i}"] = seriesStartTime;

                i++;
            }

            return null;
        }

        public async Task<RedirectResult?> OnPost([FromQuery] String matchId)
        {
            List<string> checkStrings = new List<string>
            {
                Request.Form["date"].ToString(),
                Request.Form["startTime"].ToString(),
                Request.Form["endTime"].ToString(),
                Request.Form["location"].ToString(),
                Request.Form["result"].ToString(),
                Request.Form["inner10s"].ToString(),
                Request.Form["temperature"].ToString(),
                Request.Form["humidity"].ToString(),
                Request.Form["airPressure"].ToString(),
                Request.Form["mood"].ToString()
            };

            TempData["date"] = Request.Form["date"].ToString();
            TempData["startTime"] = Request.Form["startTime"].ToString();
            TempData["endTime"] = Request.Form["endTime"].ToString();
            TempData["location"] = Request.Form["location"].ToString();
            TempData["result"] = Request.Form["result"].ToString();
            TempData["inner10s"] = Request.Form["inner10s"].ToString();
            TempData["temperature"] = Request.Form["temperature"].ToString();
            TempData["humidity"] = Request.Form["humidity"].ToString();
            TempData["airPressure"] = Request.Form["airPressure"].ToString();
            TempData["mood"] = Request.Form["mood"].ToString();
            TempData["notes"] = Request.Form["notes"].ToString();

            List<string> checkResults = new List<string>();
            List<string> checkDecimals = new List<string>();
            List<string> checkInners = new List<string>();

            for (int i = 1; i < 7; i++)
            {
                checkStrings.Add(Request.Form[$"startTime-{i}"].ToString());
                checkStrings.Add(Request.Form[$"endTime-{i}"].ToString());
                checkStrings.Add(Request.Form[$"result-{i}"].ToString());
                checkResults.Add(Request.Form[$"result-{i}"].ToString());
                checkStrings.Add(Request.Form[$"decimal-{i}"].ToString());
                checkDecimals.Add(Request.Form[$"decimal-{i}"].ToString());
                checkStrings.Add(Request.Form[$"inner10s-{i}"].ToString());
                checkInners.Add(Request.Form[$"inner10s-{i}"].ToString());
                TempData[$"startTime-{i}"] = Request.Form[$"startTime-{i}"].ToString();
                TempData[$"endTime-{i}"] = Request.Form[$"endTime-{i}"].ToString();
                TempData[$"result-{i}"] = Request.Form[$"result-{i}"].ToString();
                TempData[$"decimal-{i}"] = Request.Form[$"decimal-{i}"].ToString();
                TempData[$"inner10s-{i}"] = Request.Form[$"inner10s-{i}"].ToString();
                TempData[$"notes-{i}"] = Request.Form[$"notes-{i}"].ToString();
            }

            foreach (string checkString in checkStrings)
            {
                if (String.IsNullOrWhiteSpace(checkString))
                {
                    TempData["AlertDanger"] = "All fields (except for notes) must contain a value";
                    return null;
                }
            }

            foreach (string res in checkResults)
            {
                int temp = int.Parse(res);
                if (temp < 0 || temp > 100)
                {
                    TempData["AlertDanger"] = "Series result must be between 0 and 100 (border inclusive)";
                    return null;
                }
            }

            foreach (string dec in checkDecimals)
            {
                Double temp = Double.Parse(dec);
                if (temp < 0 || temp > 109)
                {
                    TempData["AlertDanger"] = "Decimal series result must be between 0 and 109 (border inclusive)";
                    return null;
                }
            }

            foreach (string inners in checkInners)
            {
                int temp = int.Parse(inners);
                if (temp < 0 || temp > 10)
                {
                    TempData["AlertDanger"] =
                        "Number of inner 10s in a series must be between 0 and 10 (border inclusive)";
                    return null;
                }
            }

            int result = int.Parse(Request.Form["result"].ToString());
            if (result < 0 || result > 600)
            {
                TempData["AlertDanger"] = "Match result must be between 0 and 600 (border inclusive)";
                return null;
            }

            int inner10s = int.Parse(Request.Form["inner10s"].ToString());
            if (inner10s < 0 || inner10s > 60)
            {
                TempData["AlertDanger"] = "Number of inner 10s in a match must be between 0 and 60 (border inclusive)";
                return null;
            }

            int humidity = int.Parse(Request.Form["humidity"].ToString());
            if (humidity < 0 || humidity > 100)
            {
                TempData["AlertDanger"] = "Humidity must be between 0 and 100 (border inclusive)";
                return null;
            }

            int airPressure = int.Parse(Request.Form["airPressure"].ToString());
            if (airPressure < 0)
            {
                TempData["AlertDanger"] = "Air pressure must be a non-negative number";
                return null;
            }

            string date = Request.Form["date"].ToString();
            string startTime = Request.Form["startTime"].ToString();
            string endTime = Request.Form["endTime"].ToString();
            string location = Request.Form["location"].ToString();
            string mood = Request.Form["mood"].ToString();
            string notes = Request.Form["notes"].ToString();
            string? userId = Request.Cookies["userId"]?.ToString();

            List<Dictionary<string, object>> seriesList = new List<Dictionary<string, object>>();
            for (int i = 1; i < 7; i++)
            {
                seriesList.Add(new Dictionary<string, object>
                {
                    {"StartTime", Request.Form[$"startTime-{i}"].ToString()},
                    {"EndTime", Request.Form[$"endTime-{i}"].ToString()},
                    {"Inner10s", int.Parse(Request.Form[$"inner10s-{i}"].ToString())},
                    {"Result", int.Parse(Request.Form[$"result-{i}"].ToString())},
                    {"Decimal", Double.Parse(Request.Form[$"decimal-{i}"].ToString())},
                    {"Notes", Request.Form[$"notes-{i}"].ToString()}
                });
            }

            Dictionary<string, object> newMatch = new Dictionary<string, object>
            {
                {"userId", userId},
                {"Notes", notes},
                {"Mood", mood},
                {"Location", location},
                {"EndTime", endTime},
                {"StartTime", startTime},
                {"Date", date},
                {"Humidity", humidity},
                {"Temperature", int.Parse(Request.Form["temperature"].ToString())},
                {"Result", result},
                {"Inner10s", inner10s},
                {"AirPressure", airPressure}
            };

            FirestoreDb db = FirestoreDb.Create("shootingdiary-orwima");
            CollectionReference matches = db.Collection("matches");

            if (String.IsNullOrWhiteSpace(matchId))
            {
                DocumentReference newMatchRef = await matches.AddAsync(newMatch);
                CollectionReference newSeriesCollectionRef = newMatchRef.Collection("series");

                for (int i = 1; i < 7; i++)
                {
                    DocumentReference newSeriesRef = newSeriesCollectionRef.Document(i.ToString());
                    await newSeriesRef.CreateAsync(seriesList[i - 1]);
                }

                DocumentSnapshot newMatchSnap = await newMatchRef.GetSnapshotAsync();
                TempData["AlertSuccess"] = "Match saved";
                return Redirect($"/MatchDetails?matchId={newMatchSnap.Id}");
            }

            DocumentReference matchRef = matches.Document(matchId);
            await matchRef.UpdateAsync(newMatch);

            CollectionReference seriesCollectionRef = matchRef.Collection("series");

            for (int i = 1; i < 7; i++)
            {
                DocumentReference newSeriesRef = seriesCollectionRef.Document(i.ToString());
                await newSeriesRef.UpdateAsync(seriesList[i - 1]);
            }

            TempData["AlertSuccess"] = "Match updated";
            return Redirect($"/MatchDetails?matchId={matchId}");
        }
    }
}

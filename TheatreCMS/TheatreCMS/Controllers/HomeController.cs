using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using TheatreCMS.Controllers;
using TheatreCMS.Models;
using TheatreCMS.ViewModels;
using System.Data.Entity;
using TheatreCMS.Areas.Subscribers.Models;
using System.Text.RegularExpressions;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Ajax.Utilities;
using TheatreCMS.Helpers;

namespace TheatreCMS.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        //display unread messages icon if any message sent to currentUser has a isviewed == null prop
        [ChildActionOnly]
        public ActionResult LoginNav()
        {
            var boolUnread = false;
            if (Request.IsAuthenticated)
            {
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                ApplicationUser currentUser = userManager.FindById(User.Identity.GetUserId());

                if (currentUser != null) //IF statement added to accommodate TheatreCMS_B version
                    boolUnread = db.Messages.Any(i => i.RecipientId == currentUser.Id && i.IsViewed == null);
                
            }
            ViewData["unread"] = boolUnread;

            return PartialView("_LoginPartial");
        }
        public ActionResult Index()
        {
            //==== LIST OF PRODUCTIONS - Filter by current and future productions
            var query = db.Productions.AsEnumerable().Where(p => p.OpeningDay > DateTime.Now || p.IsCurrentlyShowing() == true || (p.OpeningDay <= DateTime.Now && p.ClosingDay >= DateTime.Now))
                .OrderBy(p => p.OpeningDay);
            //===== ORDER PRODUCTIONS
            var orderedProductions = SortProductions(query.ToList());
            //===== PRODUCTION PICTURES - create list of productionPhotos objects for each production, randomize order of photo objects, nest productionPhotos objects list in a list, store list in ViewBag dictionary 
            var productionPhotosList = new List<List<ProductionPhotos>>();
            foreach (Production production in orderedProductions)
            {
                var photoArray = db.ProductionPhotos.Where(p => p.Production.ProductionId == production.ProductionId).ToList();
                productionPhotosList.Add(ShufflePhotos(photoArray));
                //DEBUG System.Diagnostics.Debug.WriteLine(photoArray.Count);
            }
            ViewBag.ProductionPhotosList = productionPhotosList;
            
            return View(orderedProductions);
        }

        private List<Production> SortProductions(List<Production> list)
        {
            var sorted = new List<Production>();
            var onStage = new List<Production>();
            var current = new List<Production>();
            var comingSoon = new List<Production>();

            foreach (var item in list)
            {
                if (item.OpeningDay <= DateTime.Now && item.ClosingDay >= DateTime.Now)
                {
                    onStage.Add(item);
                }
            }
            sorted.AddRange(onStage);
            foreach (var item in list)
            {
                if (item.IsCurrentlyShowing() && !sorted.Contains(item))
                {
                    current.Add(item);
                }
            }
            sorted.AddRange(current);
            foreach (var item in list)
            {
                if (item.OpeningDay > DateTime.Now && !sorted.Contains(item))
                {
                    comingSoon.Add(item);
                }
            }
            sorted.AddRange(comingSoon);

            return sorted;
        }

        //===== SHUFFLE PHOTOS - takes a list of ProductionPhoto objects and shuffles them
        private static readonly Random random = new Random();
        private List<ProductionPhotos> ShufflePhotos(List<ProductionPhotos> photos)
        {
            int n = photos.Count;
            while (n > 1)
            {
                n--;
                int rnd = random.Next(n + 1);
                ProductionPhotos value = photos[rnd];
                photos[rnd] = photos[n];
                photos[n] = value;
            }
            return photos;
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            var castMembers = from p in db.CastMembers
                              select p;

            ViewBag.CastMembers = castMembers.ToList();

            var dbAwards = new ApplicationDbContext();
            var awards = dbAwards.Awards.ToList();

            //grabbing all items in the DisplayLinks table in the database and putting them into the 
            //ViewBags to be displayed on the About page
            var newLink = from l in db.DisplayLinks
                          select l;

            var linkList = newLink.ToList();

            ViewBag.Buttons = (linkList.Where(x => x.Name.Contains("Buttons")));

            ViewBag.DisplayLinks = (linkList.Where(x => x.Name.Contains("Dining")));

            //taking all items in the DisplayInfo table and inserting them into the new infoList, where the ViewData will display 
            //corresponding contents on the About page
            var newInfo = from l in db.DisplayInfo
                          select l;
            var info = newInfo.ToList();
            ViewData["History"] = (info.Where(x => x.Title.Contains("History")).FirstOrDefault().TextContent);
            ViewData["Mission"] = (info.Where(x => x.Title.Contains("Mission")).FirstOrDefault().TextContent);
            

            return View(awards);
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


        //File upload GET and POST controls
        public ActionResult UploadImage()
        {

            return View();
        }
        [HttpPost]
        public ActionResult UploadImage(HttpPostedFileBase file)
        {
            try
            {
                //Using Helpers.ImageUploader.ImageBytes to get the byte[] representation of the file
                //and extracting the string representation as a returned out-parameter
                string imageBase64;
                byte[] imageBytes = ImageUploadController.ImageBytes(file, out imageBase64);

                //Add the base64 representation of the image to the ViewBag to be accessed by the View
                ViewBag.ImageData = String.Format("data:image/png;base64,{0}", imageBase64);

                ViewBag.Message = "Image uploaded successfully!";
                return View();
            }

            catch
            {
                //Using this empty string for the View to trigger when an upload fails
                ViewBag.ImageData = "";
                ViewBag.Message = "There was an error uploading your image :(";
            }
            return View();
        }

        public ActionResult NewsletterSubscribers()
        {
            using (var db = new ApplicationDbContext())
            {
                var listSubscribers = db.Subscribers.Where(s => s.Newsletter == true).ToList();
                var newsletterListVms = new List<NewsletterListVm>();
                foreach (var listSubscriber in listSubscribers)
                {
                    var signUpVm = new NewsletterListVm
                    {
                        FirstName = listSubscriber.SubscriberPerson.FirstName,
                        LastName = listSubscriber.SubscriberPerson.LastName,
                        StreetAddress = listSubscriber.SubscriberPerson.StreetAddress,
                        City = listSubscriber.SubscriberPerson.City,
                        State = listSubscriber.SubscriberPerson.State,
                        ZipCode = listSubscriber.SubscriberPerson.ZipCode,
                        Email = listSubscriber.SubscriberPerson.Email
                    };
                    newsletterListVms.Add(signUpVm);
                }
                return View(newsletterListVms);
            }

        }

        public ActionResult Archive()
        {
            var db = new ApplicationDbContext();
            var displayinfo = db.DisplayInfo.Where(x => x.Title.Contains("Archive Page"));
            string archiveTitle = displayinfo.Where(x => x.Title == "Archive Page-title").FirstOrDefault().TextContent;
            string archiveSubtitle = displayinfo.Where(x => x.Title == "Archive Page-content").FirstOrDefault().TextContent;
            ViewData["maintitle"] = archiveTitle;
            ViewData["subtitle"] = archiveSubtitle;
            var productions = db.Productions
                .Include(i => i.DefaultPhoto);
            return View(productions.ToList());
        }
        public ActionResult SignIn()
        {
            ViewBag.Message = "Your Sign In  page.";

            return View();
        }
        [HttpPost]
        //[HandleError]  This annotation can be used when custom errors is enabled. Its purpose is to protect from injection attacks.
        public ActionResult Archive(string SearchByCategory, string searchKey)
        {
            var db = new ApplicationDbContext();
            var productions = db.Productions
                .Include(i => i.DefaultPhoto);

            using (var dbSearch = new ApplicationDbContext() )
            {
                ArchiveSearch(dbSearch, SearchByCategory, searchKey);
            }
            return View(productions.ToList());
        }

        private void ArchiveSearch(ApplicationDbContext db, string searchByCategory, string searchKey)
        {
            ViewBag.Category = searchByCategory;
            if (searchKey == "")
            {
                return;
            }
            string highlightedKey = "<span id='highlight'>$&</span>";   //highlightedKey is where the css id is applied to the highlighted word.  $& swaps the search key with the original text to keep the casing intact.
            string pattern = string.Format(searchKey);                                // For whole word search, pattern = @"\b" + searchKey + @"\b"; For substring matching, pattern = searchkey;
            pattern = Regex.Escape(pattern);
            Regex rx = new Regex(pattern, RegexOptions.IgnoreCase);
            switch (searchByCategory)
            {
                case "SearchAll": //This case searches across all three tables.
                    ViewBag.Message = string.Format("Results for \"{0}\" in Archive", searchKey);
                    var resultsCast = new List<CastMember>();
                    foreach (CastMember castMember in db.CastMembers)
                    {
                        try // try catch added to handle the null Exception so it throws an exception to the console but still conducts the search.
                        {
                            Match matchName = rx.Match(castMember.Name);
                            Match matchYearJoined = rx.Match(castMember.YearJoined.ToString());
                            Match matchBio = rx.Match(castMember.Bio);
                            if (matchName.Success || matchYearJoined.Success || matchBio.Success)
                            {
                                resultsCast.Add(castMember);
                            }
                        }
                        catch (Exception e)
                        {
                        }
                    }
                    resultsCast = resultsCast.Distinct().ToList();//Prevents duplicate listings
                    if (resultsCast.Count > 0) ViewBag.ResultsCast = resultsCast;

                    var resultsProduction = new List<Production>();
                    foreach (Production production in db.Productions)
                    {
                        try // try catch added to handle the null Exception so it throws an exception to the console but still conducts the search.
                        {
                            Match matchTitle = rx.Match(production.Title);
                            Match matchPlaywright = rx.Match(production.Playwright);
                            Match matchDescription = rx.Match(production.Description);
                            if (matchTitle.Success || matchPlaywright.Success || matchDescription.Success)
                            {
                                resultsProduction.Add(production);
                            }
                        }
                        catch (Exception e)
                        {
                        }
                    }
                    resultsProduction = resultsProduction.Distinct().ToList();
                    if (resultsProduction.Count > 0) ViewBag.ResultsProduction = resultsProduction;

                    var resultsPart = new List<Part>();
                        foreach (Part part in db.Parts.ToList())
                        {
                            try // try catch added to handle the null Exception so it throws an exception to the console but still conducts the search.
                            {
                                Match matchCharacter = rx.Match(part.Character);
                                Match matchTitle = rx.Match(part.Production.Title);
                                Match matchName = rx.Match(part.Person.Name);
                                if (matchCharacter.Success || matchTitle.Success || matchName.Success)
                                {
                                    resultsPart.Add(part);
                                }
                            }
                            catch (Exception e)
                            {
                            }
                        }
                        resultsPart = resultsPart.Distinct().ToList();
                    if (resultsPart.Count > 0) ViewBag.ResultsPart = resultsPart;  

                    Highlight(resultsCast, resultsProduction, resultsPart, pattern, highlightedKey); //applies highlight effect to search matches


                    //Below is the original implementation using LINQ. I wasn't able to get it to only return results for whole words so I switched to a different implementation using Regex

                    //var resultsCast = db.CastMembers.Where(x => x.Name.ToString().Contains(searchKey.ToLower()) //Creates a list of cast members where there are search matches in any of those three columns
                    //                                         || x.YearJoined.ToString().Contains(searchKey.ToLower())
                    //                                         || x.Bio.ToLower().Contains(searchKey.ToLower())).ToList();
                    //resultsCast = resultsCast.Distinct().ToList();
                    //Highlight(resultsCast, pattern, highlightedKey);


                    //var resultsProduction = db.Productions.Where(x => x.Title.ToLower().Contains(searchKey.ToLower())
                    //                                               || x.Playwright.ToLower().Contains(searchKey.ToLower())
                    //                                               || x.Description.ToLower().Contains(searchKey.ToLower())).ToList();
                    //resultsProduction = resultsProduction.Distinct().ToList();
                    //Highlight(resultsProduction, pattern, highlightedKey);

                    //var resultsPart = db.Parts.Where(x => x.Character.ToLower().Contains(searchKey.ToLower())
                    //                                   || x.Production.Title.ToLower().Contains(searchKey.ToLower())
                    //                                   || x.Person.Name.ToLower().Contains(searchKey.ToLower())).ToList();
                    //resultsPart = resultsPart.Distinct().ToList();
                    //Highlight(resultsPart, pattern, highlightedKey);


                    //if (resultsCast.Count > 0) ViewBag.ResultsCast = resultsCast;                       //sets ViewData value if there were any results
                    //if (resultsProduction.Count > 0) ViewBag.ResultsProduction = resultsProduction;
                    //if (resultsPart.Count > 0) ViewBag.ResultsPart = resultsPart;
                    break;

                case "SearchCastMembers":
                    ViewBag.Message = string.Format("Results for \"{0}\" in Cast Members", searchKey);

                    resultsCast = new List<CastMember>();
                    foreach (CastMember castMember in db.CastMembers)
                    {
                        try // try catch added to handle the null Exception so it throws an exception to the console but still conducts the search.
                        {
                            Match matchName = rx.Match(castMember.Name);
                            Match matchYearJoined = rx.Match(castMember.YearJoined.ToString());
                            Match matchBio = rx.Match(castMember.Bio);
                            if (matchName.Success || matchYearJoined.Success || matchBio.Success)
                            {
                                resultsCast.Add(castMember);
                            }
                        }
                        catch (Exception e)
                        {
                        }
                    }
                    resultsCast = resultsCast.Distinct().ToList();//Prevents duplicate listings
                    Highlight(resultsCast, pattern, highlightedKey); //Applies highlight effect to matches
                    if (resultsCast.Count > 0) ViewBag.ResultsCast = resultsCast;                       //sets ViewData value if there were any results
                    break;

                case "SearchProductions":
                    resultsProduction = new List<Production>();
                    foreach (Production production in db.Productions)
                    {
                        try // try catch added to handle the null Exception so it throws an exception to the console but still conducts the search.
                        {
                            Match matchTitle = rx.Match(production.Title);
                            Match matchPlaywright = rx.Match(production.Playwright);
                            Match matchDescription = rx.Match(production.Description);
                            if (matchTitle.Success || matchPlaywright.Success || matchDescription.Success)
                            {
                                resultsProduction.Add(production);
                            }
                        }
                        catch (Exception e)
                        {
                        }
                    }
                    resultsProduction = resultsProduction.Distinct().ToList();
                    Highlight(resultsProduction, pattern, highlightedKey);
                    if (resultsProduction.Count > 0) ViewData["ResultsProduction"] = resultsProduction;
                    break;

                case "SearchParts":
                    ViewBag.Message = string.Format("Results for \"{0}\" in Parts", searchKey);
                    resultsPart = new List<Part>();
                    foreach (Part part in db.Parts.ToList())
                    {
                        try // try catch added to handle the null Exception so it throws an exception to the console but still conducts the search.
                        {
                            Match matchCharacter = rx.Match(part.Character);
                            Match matchTitle = rx.Match(part.Production.Title);
                            Match matchName = rx.Match(part.Person.Name);
                            if (matchCharacter.Success || matchTitle.Success || matchName.Success)
                            {
                                resultsPart.Add(part);
                            }
                        }
                        catch (Exception e)
                        {
                        }
                    }
                    resultsPart = resultsPart.Distinct().ToList();
                    Highlight(resultsPart, pattern, highlightedKey);
                    if (resultsPart.Count > 0) ViewData["ResultsPart"] = resultsPart;
                    break;
                default:
                    break;
            }
        }

        // This method has overloads for passing in different list types
        //It works by wrapping the search key in a span tag that styles it differently from the rest of the text

        private void Highlight(List<CastMember> resultsCast, List<Production> resultsProduction, List<Part> resultsPart, string pattern, string highlightedKey)
        {
            var yearJoinedString = new List<string>();
            for (int i = 0; i < resultsCast.Count; i++)   //YearJoined must be converted to text to highlight it properly. A separate list is created, then added to the viewbag.
            {
                resultsCast[i].Name = Regex.Replace(resultsCast[i].Name, pattern, highlightedKey, RegexOptions.IgnoreCase);
                resultsCast[i].Bio = Regex.Replace(resultsCast[i].Bio, pattern, highlightedKey, RegexOptions.IgnoreCase);
                yearJoinedString.Add(resultsCast[i].YearJoined.ToString());
                yearJoinedString[i] = Regex.Replace(yearJoinedString[i], pattern, highlightedKey, RegexOptions.IgnoreCase);
            }
            ViewBag.YearJoined = yearJoinedString;
            foreach (Production production in resultsProduction)
            {
                production.Title = Regex.Replace(production.Title, pattern, highlightedKey, RegexOptions.IgnoreCase);
                production.Playwright = Regex.Replace(production.Playwright, pattern, highlightedKey, RegexOptions.IgnoreCase);
                production.Description = Regex.Replace(production.Description, pattern, highlightedKey, RegexOptions.IgnoreCase);
            }
            foreach (Part part in resultsPart)
            {
                part.Character = Regex.Replace(part.Character, pattern, highlightedKey, RegexOptions.IgnoreCase);
                if (!part.Production.Title.Contains("span id=")) // these if statements prevent the span tag from being applied twice. 
                {
                    part.Production.Title = Regex.Replace(part.Production.Title, pattern, highlightedKey, RegexOptions.IgnoreCase);
                }
                if (!part.Person.Name.Contains("span id="))
                {
                    part.Person.Name = Regex.Replace(part.Person.Name, pattern, highlightedKey, RegexOptions.IgnoreCase);
                }
            }
        }

        private void Highlight(List<CastMember> resultsCast, string pattern, string highlightedKey)
        {
            var yearJoinedString = new List<string>();
            for (int i = 0; i < resultsCast.Count; i++)   //YearJoined must be converted to text to highlight it properly. A separate list is created, then added to the viewbag.
            {
                resultsCast[i].Name = Regex.Replace(resultsCast[i].Name, pattern, highlightedKey, RegexOptions.IgnoreCase);
                resultsCast[i].Bio = Regex.Replace(resultsCast[i].Bio, pattern, highlightedKey, RegexOptions.IgnoreCase);
                yearJoinedString.Add(resultsCast[i].YearJoined.ToString());
                yearJoinedString[i] = Regex.Replace(yearJoinedString[i], pattern, highlightedKey, RegexOptions.IgnoreCase);
            }
            ViewBag.YearJoined = yearJoinedString;
        }

        private void Highlight(List<Production> resultsProduction, string pattern, string highlightedKey)
        {
            foreach (Production production in resultsProduction)
            {
                production.Title = Regex.Replace(production.Title, pattern, highlightedKey, RegexOptions.IgnoreCase);
                production.Playwright = Regex.Replace(production.Playwright, pattern, highlightedKey, RegexOptions.IgnoreCase);
                production.Description = Regex.Replace(production.Description, pattern, highlightedKey, RegexOptions.IgnoreCase);
            }
        }

        private void Highlight(List<Part> resultsPart, string pattern, string highlightedKey)
        {
            foreach (Part part in resultsPart)
            {
                part.Character = Regex.Replace(part.Character, pattern, highlightedKey, RegexOptions.IgnoreCase);
                part.Production.Title = Regex.Replace(part.Production.Title, pattern, highlightedKey, RegexOptions.IgnoreCase);
                part.Person.Name = Regex.Replace(part.Person.Name, pattern, highlightedKey, RegexOptions.IgnoreCase);
            }
        }

    }
}

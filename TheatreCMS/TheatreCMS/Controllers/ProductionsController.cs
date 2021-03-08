using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;
using TheatreCMS.Helpers;
using TheatreCMS.Models;
using TheatreCMS.ViewModels;

namespace TheatreCMS.Controllers
{
    public class ProductionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        // GET: Productions
        public ActionResult Index(string searchString, string season, string month, string day, string showtime, string runtime, bool? worldPremiere, bool? isSearching, string calBool)
        {
            var productions = from p in db.Productions
                              select p;
            var showTimes = from p in db.Productions
                            select new ShowtimeListVM
                            {
                                ShowTimeEve = p.ShowtimeEve,
                                ShowTimeMat = p.ShowtimeMat
                            };

            var runTimes = from p in db.Productions
                           select p.Runtime;

            //Pass Variables to ViewBag 
            ViewBag.SeasonYears = SeasonYears();            //Dictionary of Season Years <int, string>
            ViewBag.ShowTimes = ShowtimeSort(showTimes);    //Sort showtimes ascending
            ViewBag.RunTimes = RuntimeSort(runTimes);       //Sort runtimes ascending
            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentSeason = season;
            ViewBag.CurrentMonth = month;
            ViewBag.CurrentDay = day;
            ViewBag.CurrentShowtime = showtime;
            ViewBag.CurrentRuntime = runtime;
            ViewBag.WorldPremiere = worldPremiere;

            if (!String.IsNullOrEmpty(month) && month != "any" && !String.IsNullOrEmpty(day) && day != "any") // If day && month HasValue // If day && month HasValue
            {
                var monthInt32 = Int32.Parse(month);
                monthInt32 += 1;                        // Add one to month for db comparison, value passed from view is 0-11 with 0:Jan 1:Feb etc...
                var dayInt32 = Int32.Parse(day);
                if (!IsValidDay(dayInt32, monthInt32))  // Call day validation check
                {
                    ViewBag.DayEx = "Invalid Date";
                    isSearching = true;
                    ViewBag.IsSearching = isSearching;
                    return View(productions.ToList());
                }
                else
                {
                    productions = productions.Where(p => p.OpeningDay.Month <= monthInt32 && p.ClosingDay.Month >= monthInt32);
                    productions = productions.Where(p => p.OpeningDay.Day <= dayInt32 && p.ClosingDay.Day >= dayInt32);
                    isSearching = true;

                }
            }
            else
            {
                if (!String.IsNullOrEmpty(month) && month != "any")
                {
                    var monthInt32 = Int32.Parse(month);
                    monthInt32 += 1;
                    productions = productions.Where(p => p.OpeningDay.Month <= monthInt32 && p.ClosingDay.Month >= monthInt32);
                    isSearching = true;
                }
                if (!String.IsNullOrEmpty(day) && day != "any")
                {
                    var dayInt32 = Int32.Parse(day);
                    productions = productions.Where(p => p.OpeningDay.Day <= dayInt32 && p.ClosingDay.Day >= dayInt32);
                    isSearching = true;
                }
            }
            //Compare values passed from view to values in productions
            if (!String.IsNullOrEmpty(searchString))
            {
                productions = productions.Where(p => p.Title.Contains(searchString) || p.Playwright.Contains(searchString) || p.Description.Contains(searchString));
                isSearching = true;
            }
            if (!String.IsNullOrEmpty(season) && season != "any")
            {
                var seasonInt32 = Int32.Parse(season);
                productions = productions.Where(p => p.Season == seasonInt32);
                isSearching = true;
            }
            if (!String.IsNullOrEmpty(showtime) && showtime != "any")
            {
                var dt = DateTime.Parse(showtime);
                var time = dt.TimeOfDay;
                productions = productions.Where(p => DbFunctions.CreateTime(((DateTime)p.ShowtimeEve).Hour, ((DateTime)p.ShowtimeEve).Minute, ((DateTime)p.ShowtimeEve).Second) == time || DbFunctions.CreateTime(((DateTime)p.ShowtimeMat).Hour, ((DateTime)p.ShowtimeMat).Minute, ((DateTime)p.ShowtimeMat).Second) == time);
                isSearching = true;
            }
            if (!String.IsNullOrEmpty(runtime) && runtime != "any")
            {
                var runtimeInt32 = Int32.Parse(runtime);
                productions = productions.Where(p => p.Runtime == runtimeInt32);
                isSearching = true;
            }
            if (worldPremiere == true)
            {
                productions = productions.Where(p => p.IsWorldPremiere == true);
                isSearching = true;
            }

            if (isSearching.HasValue && isSearching == true) //Pass isSearching boolean to ViewBag
            {
                ViewBag.IsSearching = isSearching;
            }
            else if (season == "any" || month == "any" || day == "any" || showtime == "any" || runtime == "any")
            {
                ViewBag.IsSearching = true;
            }
            else
            {
                ViewBag.IsSearching = false;
            }

            ViewBag.Results = productions.Count();  //Total search results

            //check if calendar is displayed before this GET Request
            ViewData["calDisplay"] = calBool;

            var prodId = productions.Select(i => i.ProductionId).ToList();

            //return only calendar events that are associated with the filtered results
            var eventList = db.CalendarEvent.Where(x => prodId.Contains(x.ProductionId ?? 0)).ToList();

            var eventArray = eventList.Select(x => new[]
            {
                x.EventId.ToString(),
                x.Title,
                x.StartDate.ToString("o"),
                x.EndDate.ToString("o"),
                x.TicketsAvailable.ToString(),
                x.Color,
                x.ProductionId.ToString()
            }).ToArray();

            ViewData["events"] = eventArray;

            return View(productions.OrderByDescending(p => p.OpeningDay).ToList());
        }

        public ActionResult Current()
        {
            var current = from a in db.Productions
                          .ToList()
                          where  a.IsCurrentlyShowing()
                          select a;
            return View(current);
        }

        // GET: Productions/Details/id
        public ActionResult Details(int? id)
        {

            AdminSettings adminSettings = AdminSettingsReader.CurrentSettings();

            if (id == null)
            {
                id = adminSettings.on_stage;
            }
            Production production = db.Productions.Find(id);
            if (production == null)
            {
                return HttpNotFound();
            }
            List<int> positions = new List<int>() { 0, 1, 3, 2, 4 };
            ViewBag.Positions = positions;
            return View(production);
        }

        //------------------------------------------------------
        //----------------------- CREATE -----------------------
        //------------------------------------------------------
        
        //========== GET: Productions/Create
        [TheatreAuthorize(Roles = "Admin")]
        public ActionResult Create()
        {
            ViewData["current_season"] = AdminSettingsReader.CurrentSettings().current_season;
            return View();
        }

        //========== POST: Productions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [TheatreAuthorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductionId,Title,Playwright,Description,Runtime,OpeningDay,ClosingDay,DefaultPhoto,ShowtimeEve,ShowtimeMat,TicketLink,Season,IsCurrent,IsWorldPremiere")] Production production, HttpPostedFileBase uploadFile)
        {
            //========== VALIDATION ==========
            //===== PHOTO - Check if photo is not null but not a valid photo format
            if (uploadFile != null && !PhotoController.ValidatePhoto(uploadFile))
            {
                ModelState.AddModelError("DefaultPhoto", "File must be a valid photo format.");                
            }
            //===== SHOWTIME - at list one (ShowtimeEve, ShowtimeMat) need to be assigned 
            if (production.ShowtimeEve == null && production.ShowtimeMat == null)
            {
                ModelState.AddModelError("ShowtimeEve", "At least one showtime must be specified.");
                ModelState.AddModelError("ShowtimeMat", "At least one showtime must be specified.");
            }
            //========== SAVE ==========
            if (ModelState.IsValid)
            {
                //===== DEFAULT PHOTO 
                //--- save photo using photo controller, save entry to ProductionPhotos, photoName set to production title, description set to "Default Photo"
                if (uploadFile != null)
                {
                    //----- Save New Photo or retrieve existing photo if the same - using photo controller
                    Debug.WriteLine("Saving photo...");
                    int photoId = PhotoController.CreatePhoto(uploadFile, production.Title);
                    //----- Save New ProductionPhoto
                    var productionPhoto = new ProductionPhotos() { PhotoId = photoId, Title = production.Title, Description = "Default Photo" };
                    db.ProductionPhotos.Add(productionPhoto);
                    db.SaveChanges();
                    //----- Save New Production, add DefaultPhoto object reference to production
                    production.DefaultPhoto = productionPhoto;
                    db.Productions.Add(production);
                    db.SaveChanges();
                    //----- Add Production object reference to productionPhoto
                    productionPhoto.Production = production;
                    db.Entry(productionPhoto).State = EntityState.Modified;
                    db.SaveChanges();
                }
                //===== NO PHOTO 
                else
                {
                    db.Productions.Add(production);
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            return View(production);
        }


        // GET: Productions/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Production production = db.Productions.Find(id);
            if (production == null)
            {
                return HttpNotFound();
            }

            ViewData["ProductionPhotos"] = new SelectList(db.ProductionPhotos.Where(x => x.Production.ProductionId == id).ToList(), "ProPhotoId", "Title");
            return View(production);
        }


        // POST: Productions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles  = "Admin")]
        public ActionResult Edit([Bind(Include = "ProductionId,DefaultProPhotoId,Title,Playwright,Description,Runtime,OpeningDay,ClosingDay,DefaultPhoto,ShowtimeEve,ShowtimeMat,TicketLink,Season,IsCurrent,IsWorldPremiere")] Production production, HttpPostedFileBase upload)
        {

            if (ModelState.IsValid)  //!!!!!part controller
            {
                var currentProduction = db.Productions.Find(production.ProductionId);
                currentProduction.Title = production.Title;
                currentProduction.Playwright = production.Playwright;
                currentProduction.Description = production.Description;
                currentProduction.Runtime = production.Runtime;
                currentProduction.OpeningDay = production.OpeningDay;
                currentProduction.ClosingDay = production.ClosingDay;
                currentProduction.ShowtimeEve = production.ShowtimeEve;
                currentProduction.ShowtimeMat = production.ShowtimeMat;
                currentProduction.TicketLink = production.TicketLink;
                currentProduction.Season = production.Season;
                //currentProduction.IsCurrentlyShowing = production.IsCurrentlyShowing;
                currentProduction.IsWorldPremiere = production.IsWorldPremiere;

               
                if (int.TryParse(Request.Form["DefaultProPhotoId"], out int defaultProPhotoId))
                {
                    var productionPhoto = db.ProductionPhotos.Find(defaultProPhotoId);
                    currentProduction.DefaultPhoto = productionPhoto;

                    // Ignoring attempt to update photo specifically until model can handle null values.
                    try
                    {
                        db.Entry(currentProduction.DefaultPhoto).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    catch (System.ArgumentNullException)
                    {
                        //Allowing this argument to pass
                    }
                }
                
                

                db.Entry(currentProduction).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(production);
        }


        // GET: Productions/Delete/id
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Production production = db.Productions.Find(id);
            if (production == null)
            {
                return HttpNotFound();
            }
            return View(production);
        }

        // Productions/Delete/id
        public ActionResult DeleteProduction(int id)
        {
            Production prod = db.Productions.Find(id);
            Part[] parts = prod.Parts.ToArray();
            CalendarEvent[] calEvents = prod.Events.ToArray();
            ProductionPhotos[] productionPhotos = prod.ProductionPhotos.ToArray();
            IQueryable prodAwards = db.Awards.Where(a => a.ProductionId == prod.ProductionId);

           
            foreach (Award prodAward in prodAwards) db.Awards.Remove(prodAward);
            foreach (Part part in parts) db.Parts.Remove(part);
            foreach (CalendarEvent calEvent in calEvents) db.CalendarEvent.Remove(calEvent);
            foreach (ProductionPhotos prodPhoto in productionPhotos) db.ProductionPhotos.Remove(prodPhoto);
            db.SaveChanges();

            db.Productions.Remove(prod);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // Productions/Delete/id including all dependencies
        public ActionResult DeleteProductionWithPhotos(int id)
        {

            Production prod = db.Productions.Find(id);
            ProductionPhotos[] productionPhotos = prod.ProductionPhotos.ToArray();
            Part[] parts = prod.Parts.ToArray();
            CalendarEvent[] calEvents = prod.Events.ToArray();
            IQueryable prodAwards = db.Awards.Where(a => a.ProductionId == prod.ProductionId);

            var photos = new List<Photo>();
            for (int i = 0; i < productionPhotos.Length; i++)
            {
                int? photoId = productionPhotos[i].PhotoId;
                Photo photoQuery = db.Photo.Find(photoId);
                photos.Add(photoQuery);
            }

            foreach (ProductionPhotos prodPhoto in productionPhotos) db.ProductionPhotos.Remove(prodPhoto);
            foreach (Part part in parts) db.Parts.Remove(part);
            foreach (CalendarEvent calEvent in calEvents) db.CalendarEvent.Remove(calEvent);
            foreach (Award prodAward in prodAwards) db.Awards.Remove(prodAward);
            db.SaveChanges();

            foreach (Photo photo in photos) db.Photo.Remove(photo);
            db.SaveChanges();

            db.Productions.Remove(prod);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private Dictionary<int, string> SeasonYears()
        {
            var currentSeason = AdminSettingsReader.CurrentSettings().current_season;
            int latestYear = DateTime.Now.Year;
            int currentMonth = DateTime.Now.Month;
            if (currentMonth >= 6) //Seasons last from June - June, if True current season must display latest year as current year + 1
            {
                latestYear++;
            }

            Dictionary<int, string> seasonYears = new Dictionary<int, string>(); //Create dictionary file from current season in json file
            for (int i = currentSeason; i > 0; i--)
            {
                string year = latestYear.ToString();
                string previousYear = (latestYear - 1).ToString();
                if (i == currentSeason)
                {
                    seasonYears.Add(i, "Current ('" + previousYear.Substring(year.Length - 2) + " ~ '" + year.Substring(previousYear.Length - 2) + ")");
                }
                else
                {
                    seasonYears.Add(i, i + " ('" + previousYear.Substring(year.Length - 2) + " ~ '" + year.Substring(previousYear.Length - 2) + ")");
                }
                latestYear--;
            }
            return seasonYears;
        }

        public bool IsValidDay(int day, int month)      //Date Validator
        {
            var year = DateTime.Now.Year;
            bool b = false;
            var currentSeason = SeasonYears().Count();

            for (int i = currentSeason; i >= 0; i--)        //Using currentseason, loop through theatre years, if dd/mm exists in any of those years, bool is valid.
            {
                if (day <= DateTime.DaysInMonth(year, month))
                {
                    b = true;
                }
                year--;
            }
            return b;
        }

        public List<string> ShowtimeSort(IEnumerable<ShowtimeListVM> showtimeListVMs)   //Sort Method
        {
            var unorderedShowtimeList = new List<DateTime>();
            foreach (var item in showtimeListVMs.ToList())      //loop through list of ShowtimeListVM containing values of ShowTimeEve/Mat
            {
                if (item.ShowTimeEve.HasValue)
                {
                    unorderedShowtimeList.Add((DateTime)item.ShowTimeEve);
                }
                if (item.ShowTimeMat.HasValue)
                {
                    unorderedShowtimeList.Add((DateTime)item.ShowTimeMat);
                }
            }
            var orderedShowtimeList = unorderedShowtimeList.OrderBy(x => x.TimeOfDay).ToList();     //Sort list ascending

            var showTimes = new List<String>();
            foreach (var item in orderedShowtimeList)       //Convert each value to string in format "hh:mm am/pm"
            {
                if (!showTimes.Contains(item.ToShortTimeString()))          //Check if showTimes list contains showtime already
                {
                    showTimes.Add(item.ToShortTimeString());
                }
            }
            return showTimes;
        }

        public List<int> RuntimeSort(IEnumerable<int> runTimesList) //Sort method
        {
            var runTimes = new List<int>(); //Sort runtimes in ascending order
            foreach (var item in runTimesList.ToList())
            {
                if (!runTimes.Contains(item))
                {          //Check if runTimes list contains runtime already
                    runTimes.Add(item);
                }
            }
            runTimes.Sort();
            return runTimes;
        }

        //Add photo while editing production if no default photo exists
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddProductionPhoto([Bind(Include = "Title,Description")] ProductionPhotos productionPhotos, HttpPostedFileBase file, int productionId)
        {

            productionPhotos.PhotoId = PhotoController.CreatePhoto(file, productionPhotos.Title);

            if (ModelState.IsValid)
            {
                Production production = db.Productions.Find(productionId);
                productionPhotos.Production = production;

                if (production.DefaultPhoto == null)
                {
                    production.DefaultPhoto = productionPhotos;
                }

                db.ProductionPhotos.Add(productionPhotos);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            return View("Edit");
        }
    }

    public static class IntegerExtensions   //Integer extensions class and method
    {
        public static string DisplayWithSuffix(this int num)
        {
            string number = num.ToString();
            if (number.EndsWith("11")) return number + "th";
            if (number.EndsWith("12")) return number + "th";
            if (number.EndsWith("13")) return number + "th";
            if (number.EndsWith("1")) return number + "st";
            if (number.EndsWith("2")) return number + "nd";
            if (number.EndsWith("3")) return number + "rd";
            return number + "th";
        }
    }
}

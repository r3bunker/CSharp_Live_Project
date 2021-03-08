using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Windows.Documents;
using TheatreCMS.Models;
using System.Web.Mvc.Html;
using System.Web.UI.WebControls;
using System.Diagnostics;
using System.Web.Script.Serialization;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;

namespace TheatreCMS.Controllers
{
    public class CalendarEventsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: CalendarEvents
        public ActionResult Index()
        {
            ViewData["Productions"] = new SelectList(db.Productions.ToList(), "ProductionId", "Title");
            return View(db.CalendarEvent.ToList());
        }

        public JsonResult GetCalendarEvents()
        {
            var events = db.CalendarEvent.ToArray();

            return Json(db.CalendarEvent.Select(x => new
            {
                id = x.EventId,
                title = x.Title,
                start = x.StartDate,
                end = x.EndDate,
                seats = x.TicketsAvailable,
                color = x.Color,
                className = x.ClassName,
                someKey = x.SomeKey,
                allDay = false,
                productionid = x.ProductionId,//added for calendar event modals
            }).ToArray(), JsonRequestBehavior.AllowGet);
        }

        // GET: CalendarEvents/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CalendarEvent calendarEvent = db.CalendarEvent.Find(id);
            if (calendarEvent == null)
            {
                return HttpNotFound();
            }
            return View(calendarEvent);
        }

        // GET: CalendarEvents/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {

            ViewData["Productions"] = new SelectList(db.Productions.ToList(), "ProductionId", "Title");
            return View();


        }

        // POST: CalendarEvents/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for  on, let me show u the 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Create([Bind(Include = "EventId,Title,StartDate,EndDate,TicketsAvailable,Color,ProductionId")] CalendarEvent calendarEvent)
        {          
           var isAjax = Request.IsAjaxRequest();
            if (ModelState.IsValid && !isAjax)
            {
                ViewData["Productions"] = new SelectList(db.Productions.ToList(), "ProductionId", "Title");

                var productionID = Request.Form["Productions"];

                if (ViewData["Productions"] != null)
                    db.CalendarEvent.Add(calendarEvent);

                db.SaveChanges();
                //System.Diagnostics.Debug.WriteLine("This call was NOT made from ajax, therefore from Create MVC Page. isAjax: " + isAjax);
                return RedirectToAction("Index");              
            }
            if (ModelState.IsValid && isAjax){
                
                //System.Diagnostics.Debug.WriteLine("This call was made from Ajax, therefore from Add Modal. isAjax: " + isAjax);
                db.CalendarEvent.Add(calendarEvent);
                db.SaveChanges();
                return Json(new { success = true });
            }
            return View(calendarEvent);
        }

        // GET: CalendarEvents/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CalendarEvent calendarEvents = db.CalendarEvent.Find(id);

            if (calendarEvents == null)
            {
                return HttpNotFound();
            }
            return View(calendarEvents);
        }

        // POST: CalendarEvents/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit([Bind(Include = "EventId,Title,StartDate,EndDate,TicketsAvailable,Color,ProductionId")] CalendarEvent calendarEvents)
        {
            var isAjax = Request.IsAjaxRequest();
            if (ModelState.IsValid && !isAjax)
            {                
                db.Entry(calendarEvents).State = EntityState.Modified;
                db.SaveChanges();
                System.Diagnostics.Debug.WriteLine("This call was NOT made from ajax, therefore from Create MVC Page. isAjax: " + isAjax);
                return RedirectToAction("Index");
            }
            if (ModelState.IsValid && isAjax)
            {
                System.Diagnostics.Debug.WriteLine("This call was made from Ajax, therefore from Add Modal. isAjax: " + isAjax);
                db.Entry(calendarEvents).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true });
            }
            return View();
        }

        // GET: CalendarEvents/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CalendarEvent calendarEvent = db.CalendarEvent.Find(id);
            if (calendarEvent == null)
            {
                return HttpNotFound();
            }
            return View(calendarEvent);
        }

        // POST: CalendarEvents/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CalendarEvent calendarEvent = db.CalendarEvent.Find(id);
            db.CalendarEvent.Remove(calendarEvent);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        // POST: CalendarEvents Confirm Delete Modal
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public JsonResult DeletingEvent(int id)
        {
            var status = false;
            CalendarEvent eventtodel = db.CalendarEvent.Where(a => a.EventId == id).FirstOrDefault();
            if (eventtodel != null)
            {
                db.CalendarEvent.Remove(eventtodel);
                db.SaveChanges();
                status = true;
            }
            return new JsonResult { Data = new { status = status } };
        }

        //POST: CalendarEvents Delete Multiple Events From Table
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteMultiple(FormCollection events)
        {
            var values = events["eventsArray"];
            string[] str = values.Split(new string[] {","}, StringSplitOptions.None);
            for (int i = 0; i < str.Length; i++)
            {
                CalendarEvent calendarEvent = db.CalendarEvent.Find(Int32.Parse(str[i]));
                db.CalendarEvent.Remove(calendarEvent);
                db.SaveChanges();
            }
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

        // GET: CalendarEvents/BulkAdd

        [Authorize(Roles = "Admin")]
        public ActionResult BulkAdd()
        {
            ViewData["Productions"] = new SelectList(db.Productions.OrderByDescending(x => x.Season).ToList(), "ProductionId", "Title");
            ViewData["Times"] = GetTimeIntervals();
            Debug.WriteLine("BulkAdd main");
            return View();
        }

        
        [Authorize(Roles = "Admin")]
        public ActionResult GetProduction(int productionId = 0)
        {
            int id = Convert.ToInt32(productionId);
            var query = from production in db.Productions
                        where production.ProductionId == id
                        select new { production.OpeningDay, production.ClosingDay, production.ShowtimeMat, production.ShowtimeEve, production.Runtime }; /*ShowtimeMat = production.ShowtimeMat.Value.ToString("hh:mm tt"), ShowtimeEve = production.ShowtimeEve.Value.ToString*/

            return Json(JsonConvert.SerializeObject(query), JsonRequestBehavior.AllowGet);

        }

        // This method generates times for the show time dropdown

        public List<string> GetTimeIntervals()
        {
            List<string> timeIntervals = new List<string>();
            TimeSpan startTime = new TimeSpan(8, 0, 0);                 // The first time to be added. (8,0,0) sets it to 8 am
            DateTime startDate = new DateTime(DateTime.MinValue.Ticks); // Date to be used to get shortTime format.
            timeIntervals.Add("TBD");

            for (int i = 0; i < 29; i++)                                // This loop adds times to the array in 30 min increments ending at 10 pm
            {
                int minutesToBeAdded = 30 * i;      // Increasing minutes by 30 minutes interval
                TimeSpan timeToBeAdded = new TimeSpan(0, minutesToBeAdded, 0);
                TimeSpan t = startTime.Add(timeToBeAdded);
                DateTime result = startDate + t;
                timeIntervals.Add(result.ToShortTimeString());      // Use Date.ToShortTimeString() method to get the desired format                
            }
            return timeIntervals;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult BulkAdd(string jsonString)
        {
            Debug.WriteLine("BulkAdd POST");
            if (jsonString != null && jsonString != "" && jsonString != "[\"\"]")
            {
                IList<CalendarEvent> events = JsonConvert.DeserializeObject<List<CalendarEvent>>(jsonString);
                try
                {
                db.CalendarEvent.AddRange(events);
                db.SaveChanges();
                }
                catch (System.Data.SqlClient.SqlException)
                {
                    Console.WriteLine("Something went wrong when updating the database!");
                }
            }
            // MVC doesn't support redirecting when an AJAX call is made. In order to redirect to another page, it must be done on the javascript side.
            //it can be done in the "success" property of the ajax call.
            return RedirectToAction("BulkAdd"); 
        }
    }
}

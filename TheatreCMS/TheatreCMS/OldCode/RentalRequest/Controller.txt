using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TheatreCMS.Models;

namespace TheatreCMS.Controllers
{
    public class RentalRequestController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: RentalRequest
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            return View(db.RentalRequests.ToList());
        }

        public void RentalAddCalendar(RentalRequest rentalRequest)
        {
            CalendarEvent calendar = new CalendarEvent();

            calendar.RentalRequestId = rentalRequest.RentalRequestId;
            calendar.Title = rentalRequest.Company;
            calendar.StartDate = rentalRequest.StartTime;
            calendar.EndDate = rentalRequest.EndTime;
            db.CalendarEvent.Add(calendar);
            db.SaveChanges();

        }

        public void RentalEditCalendar(RentalRequest rentalRequest)
        {

            CalendarEvent calendar = db.CalendarEvent.Where(x => x.RentalRequestId == rentalRequest.RentalRequestId).FirstOrDefault();

            if (calendar == null)
            {
                RentalAddCalendar(rentalRequest);
            }
            else if (rentalRequest.RentalRequestId == calendar.RentalRequestId)
            {
                calendar.Title = rentalRequest.Company;
                calendar.StartDate = rentalRequest.StartTime;
                calendar.EndDate = rentalRequest.EndTime;
                db.Entry(calendar).State = EntityState.Modified;
                db.SaveChanges();
            }

        }

        public void RentalDeleteCalendar(RentalRequest rentalRequest)
        {
            CalendarEvent calendar = db.CalendarEvent.Where(x => x.RentalRequestId == rentalRequest.RentalRequestId).FirstOrDefault();
            if (calendar != null)
            {
                db.CalendarEvent.Remove(calendar);
                db.SaveChanges();
            }

        }


        // GET: RentalRequest/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RentalRequest rentalRequest = db.RentalRequests.Find(id);
            if (rentalRequest == null)
            {
                return HttpNotFound();
            }
            return View(rentalRequest);
        }

        // GET: RentalRequest/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RentalRequest/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "RentalRequestId,ContactPerson,ContactPhoneNumber,ContactEmail,Company,StartTime,EndTime,ProjectInfo,Requests,RentalCode,Accepted,ContractSigned")] RentalRequest rentalRequest)
        {
            long sec = 10000000;        //DateTime ticks per second
            long hrSecs = 3600;         //Seconds in an hour
            DateTime endOfCurrentDayDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
            long endOfCurrentDate =  Convert.ToInt64(endOfCurrentDayDateTime.Ticks) / sec;
            long Strtm = Convert.ToInt64(rentalRequest.StartTime.Ticks) / sec;    //divided ticks into seconds
            long Endtm = Convert.ToInt64(rentalRequest.EndTime.Ticks) / sec;
            if (Endtm < (Strtm + hrSecs) && Endtm >= Strtm)    //Doesn't allow rentals < 1hr
            {
                ModelState.AddModelError("EndTime", "** Rental must be at least 1 hour.  **");  
            }
            if (Strtm <= endOfCurrentDate)
            {
                ModelState.AddModelError("StartTime", "Date cannot occur before current date");
            }
            if (Endtm < Strtm)   //Keeps End time after start time
            {
                ModelState.AddModelError("StartTime", "** Start Time cannot occur after End Time.  **");
            }
            if (ModelState.IsValid)
            {
                db.RentalRequests.Add(rentalRequest);
                var randomNum = new Random();
                int codeNum = randomNum.Next(10000, 99999);
                rentalRequest.RentalCode = codeNum;
                db.SaveChanges();
                if (rentalRequest.Accepted == true)
                {
                    RentalAddCalendar(rentalRequest);
                }
                // Changing redirect to RequestConfirm view since Index is for Admin only. 
                return RedirectToAction("RequestConfirm");
            }

            return View(rentalRequest);
        }

        // GET: RentalRequest/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RentalRequest rentalRequest = db.RentalRequests.Find(id);
            if (rentalRequest == null)
            {
                return HttpNotFound();
            }
            return View(rentalRequest);
        }

        // POST: RentalRequest/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit([Bind(Include = "RentalRequestId,ContactPerson,ContactPhoneNumber,ContactEmail,Company,StartTime,EndTime,ProjectInfo,Requests,Accepted,ContractSigned")] RentalRequest rentalRequest)
        {
            long sec = 10000000;        //DateTime ticks per second
            long hrSecs = 3600;         //Seconds in an hour
            long Strtm = Convert.ToInt64(rentalRequest.StartTime.Ticks) / sec;    //divided ticks into seconds
            long Endtm = Convert.ToInt64(rentalRequest.EndTime.Ticks) / sec;
            if (Endtm < (Strtm + hrSecs) && Endtm >= Strtm)    //Doesn't allow rentals < 1hr
            {
                ModelState.AddModelError("EndTime", "** Rental must be at least 1 hour.  **");
            }
            if (Endtm < Strtm)   //Keeps End time after start time
            {
                ModelState.AddModelError("StartTime", "** Start Time cannot occur after End Time.  **");
            }
            if (ModelState.IsValid)
            {

                var currentRentalRequest = db.RentalRequests.Find(rentalRequest.RentalRequestId);

                currentRentalRequest.ContactPerson = rentalRequest.ContactPerson;
                currentRentalRequest.ContactPhoneNumber = rentalRequest.ContactPhoneNumber;
                currentRentalRequest.ContactEmail = rentalRequest.ContactEmail;
                currentRentalRequest.Company = rentalRequest.Company;
                currentRentalRequest.StartTime = rentalRequest.StartTime;
                currentRentalRequest.EndTime = rentalRequest.EndTime;
                currentRentalRequest.ProjectInfo = rentalRequest.ProjectInfo;
                currentRentalRequest.Requests = rentalRequest.Requests;
                currentRentalRequest.Accepted = rentalRequest.Accepted;
                currentRentalRequest.ContractSigned = rentalRequest.ContractSigned;

                db.Entry(currentRentalRequest).State = EntityState.Modified;
                db.SaveChanges();

                if (rentalRequest.Accepted == true)
                {
                    RentalEditCalendar(rentalRequest);
                }
                else if (rentalRequest.Accepted == false)
                {
                    RentalDeleteCalendar(rentalRequest);
                }
                return RedirectToAction("Index");
            }
            return View(rentalRequest);
        }

        // GET: RentalRequest/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RentalRequest rentalRequest = db.RentalRequests.Find(id);
            if (rentalRequest == null)
            {
                return HttpNotFound();
            }
            return View(rentalRequest);
        }

        // POST: RentalRequest/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            RentalRequest rentalRequest = db.RentalRequests.Find(id);
            if (rentalRequest.Accepted == true)
            {
                RentalDeleteCalendar(rentalRequest);
            }
            db.RentalRequests.Remove(rentalRequest);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult RequestConfirm()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public JsonResult GetRentalEvents()
        {
            var events = db.RentalRequests.ToArray();

            return Json(db.RentalRequests.Select(x => new
            {
                rentalRequestId = x.RentalRequestId,
                contactPerson = x.ContactPerson,
                company = x.Company,
                start = x.StartTime,
                end = x.EndTime,               
                projectInfo = x.ProjectInfo,
                requests = x.Requests,
                rentalCode= x.RentalCode,
                accepted = x.Accepted,
                contractSigned = x.ContractSigned,
                allDay = false,
               
            }).ToArray(), JsonRequestBehavior.AllowGet);
        }

        

    }
} 


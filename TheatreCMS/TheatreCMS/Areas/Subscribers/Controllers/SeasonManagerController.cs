using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TheatreCMS.Areas.Subscribers.Models;
using TheatreCMS.Models;
using TheatreCMS.Helpers;
using System.Diagnostics;
using Microsoft.AspNet.Identity;

namespace TheatreCMS.Areas.Subscribers.Controllers
{
    public class SeasonManagerController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Subscribers/SeasonManager
        public ActionResult Index()
        {
            return View(db.SeasonManagers.ToList());
        }

        // GET: Subscribers/SeasonManager/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SeasonManager seasonManager = db.SeasonManagers.Find(id);
            if (seasonManager == null)
            {
                return HttpNotFound();
            }
            return View(seasonManager);
        }

        // GET: Subscribers/SeasonManager/Create
        public ActionResult Create()
        {
            int currentSeason = AdminSettingsReader.CurrentSettings().current_season;
            int[] validSeason = new int[] { currentSeason, currentSeason + 1 };   //Creates a list of the current season and the next season to populate the Season field
            ViewData["Season"] = new SelectList(validSeason.ToList(), validSeason, "Season");                       //
            
            if (User.IsInRole("Admin"))                                                                             // this block is used to send a list of names to the user dropdown. If the user isn't an admin, they just see their name.
            {
                ViewData["dbUsers"] = new SelectList(db.Users.ToList(), "ID", "UserName");
            }
            else
            {
                ViewData["dbUsers"] = new SelectList(db.Users.Where(name => name.UserName == User.Identity.Name) .ToList(), "ID", "UserName");
            }

            ViewBag.HasAccess = (User.IsInRole("Admin") || User.IsInRole("Subscriber")) ?  true : false;   // this sets a viewbag property which is used to disable forms based on the user's access.
            return View();
        }

        // POST: Subscribers/SeasonManager/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SeasonManagerId,Season,NumberSeats,BookedCurrent,FallProd,FallTime,BookedFall,WinterProd,WinterTime,BookedWinter,SpringProd,SpringTime,BookedSpring,SeasonManagerPerson")] SeasonManager seasonManager)
        {
            ModelState.Remove("SeasonManagerPerson");
            string userId = Request.Form["dbUsers"].ToString();

            if (ModelState.IsValid)
               
            {
                
                ViewData["dbUsers"] = new SelectList(db.Users.ToList(), "Id", "UserName");
                seasonManager.SeasonManagerPerson = db.Users.Find(userId);
                db.SeasonManagers.Add(seasonManager);               
                if (seasonManager.FallTime != null)
                {
                    seasonManager.BookedFall = true;
                }
                if (seasonManager.WinterTime != null)
                {
                    seasonManager.BookedWinter = true;
                }
                if (seasonManager.SpringTime != null)
                {
                    seasonManager.BookedSpring = true;
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(seasonManager);
        }

        // GET: Subscribers/SeasonManager/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SeasonManager seasonManager = db.SeasonManagers.Find(id);
            if (seasonManager == null)
            {
                return HttpNotFound();
            }
            ViewData["dbUsers"] = new SelectList(db.Users.ToList(), "ID", "UserName");
            return View(seasonManager);
        }

        // POST: Subscribers/SeasonManager/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SeasonManagerId,NumberSeats,BookedCurrent,FallProd,FallTime,BookedFall,WinterProd,WinterTime,BookedWinter,SpringProd,SpringTime,BookedSpring, SeasonManagerPerson")] SeasonManager seasonManager)
        {
            ModelState.Remove("SeasonManagerPerson");
            string userId = Request.Form["dbUsers"].ToString();

            if (ModelState.IsValid)
            {
                ViewData["dbUsers"] = new SelectList(db.Users.ToList(), "Id", "UserName");
                seasonManager.SeasonManagerPerson = db.Users.Find(userId);
                db.Entry(seasonManager).State = EntityState.Modified;
                if (seasonManager.FallTime != null)
                {
                    seasonManager.BookedFall = true;
                }
                if (seasonManager.WinterTime != null)
                {
                    seasonManager.BookedWinter = true;
                }
                if (seasonManager.SpringTime != null)
                {
                    seasonManager.BookedSpring = true;
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(seasonManager);
        }

        // GET: Subscribers/SeasonManager/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SeasonManager seasonManager = db.SeasonManagers.Find(id);
            if (seasonManager == null)
            {
                return HttpNotFound();
            }
            return View(seasonManager);
        }

        // POST: Subscribers/SeasonManager/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SeasonManager seasonManager = db.SeasonManagers.Find(id);
            db.SeasonManagers.Remove(seasonManager);
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
    }
}

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
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
using Owin;

namespace TheatreCMS.Areas.Subscribers.Controllers
{
    [Authorize(Roles = "Subscriber, Admin")]
    public class SubscriberController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Subscribers/Subscriber
        public ActionResult Index()
        {
            return View(db.Subscribers.ToList());
        }

        // GET: Subscribers/Subscriber/Details/5
        public ActionResult Details(string id)
        {
            
            if (id == null)
            {
                id = User.Identity.GetUserId();
            }
            Subscriber subscriber = db.Subscribers.Find(id);
            if (subscriber == null)
            {
                return HttpNotFound();
            }
            return View(subscriber);
        }

        // GET: Subscribers/Subscriber/Create
        public ActionResult Create()
        {
            //Pass data into SelectList to display for the user to choose which user subscription relates to
            
            ViewData["dbUsers"] = new SelectList(db.Users.ToList(), "Id", "UserName");
            return View();
        }

        // POST: Subscribers/Subscriber/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SubscriberId,CurrentSubscriber,HasRenewed,Newsletter,RecentDonor,LastDonated,LastDonationAmt,SpecialRequests,Notes")]  Subscriber subscriber)
        {
            //The form sent the user's User selection (from SelectList) into the POST method
            //Remove the SubscriberPerson from ModelState, at dbo.Subscribers has no such column
            ModelState.Remove("SubscriberPerson");

            //Extract the Guid as type String from user's selected User (from SelectList)
            string userId = null;

            if (User.IsInRole("Admin"))
            {
                userId = Request.Form["dbUsers"].ToString();
            }
            else 
            { 
                userId = User.Identity.GetUserId();
            }

            if (ModelState.IsValid)
            {
              
                
                //See tutorials for why SelectList is loaded here as well
                ViewData["dbUsers"] = new SelectList(db.Users.ToList(), "Id", "UserName");

                //LINQ statemenet to query the Guid (via String) of the user's selected User
                subscriber.SubscriberPerson = db.Users.Find(userId);

                //create instance of UserManager class &add user to "Subscriber" role
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));

                if (userManager.GetRoles(userId).Count < 1)
                {
                    userManager.AddToRole(userId, "Subscriber");
                }



                //Add Subscriber to database, linked with User and save changes
                try
                {
                    db.Subscribers.Add(subscriber);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch
                {
                    ViewBag.SubscriberError = "Sorry, there was an error submitting this form.";
                    return View("Create");
                }
                
               
               
              
            }
            return View(subscriber);
        }

        // GET: Subscribers/Subscriber/Edit/5
        public ActionResult Edit(string id = null)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Subscriber subscriber = db.Subscribers.Find(id);
            if (subscriber == null)
            {
                return HttpNotFound();
            }

            ViewData["dbUsers"] = new SelectList(db.Users.ToList(), "Id", "UserName");

            return View(subscriber);
        }

        // POST: Subscribers/Subscriber/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SubscriberId,CurrentSubscriber,HasRenewed,Newsletter,RecentDonor,LastDonated,LastDonationAmt,SpecialRequests,Notes")] Subscriber subscriber)
        {
            ModelState.Remove("SubscriberPerson");
            if (ModelState.IsValid)
            {
                db.Entry(subscriber).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(subscriber);
        }

        // GET: Subscribers/Subscriber/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Subscriber subscriber = db.Subscribers.Find(id);
            if (subscriber == null)
            {
                return HttpNotFound();
            }
            return View(subscriber);
        }

        // POST: Subscribers/Subscriber/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Subscriber subscriber = db.Subscribers.Find(id);
            db.Subscribers.Remove(subscriber);
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

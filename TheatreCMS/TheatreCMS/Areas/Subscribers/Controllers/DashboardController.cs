using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TheatreCMS.Areas.Subscribers.Models;
using TheatreCMS.Models;

namespace TheatreCMS.Areas.Subscribers.Controllers
{
    public class DashboardController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index()
        {
            
            string id = User.Identity.GetUserId();
            ApplicationUser user = db.Users.Find(id);
            return View(user);
        }

       //Post. Edits the signed in Users Subscription Plan. Recieves selected option from index page.
        public ActionResult ChangeSubscriptionPlan(string sublevel)
        {
            //Finds user ID and associated subsciber attribute values
            string id = User.Identity.GetUserId();
            ApplicationUser user = db.Users.Find(id);
            Subscriber subscriber = user.SubscriberPerson;

            //Seaches subscription plan model for attribute matching selected option from index page. Assigns subscription plan object to local variable
            var subscriptionplan = db.SubscriptionPlan.FirstOrDefault(p => p.SubscriptionLevel == sublevel);
            //Making changes to model and save. Returns to index page
            subscriber.SubscriptionPlan = subscriptionplan;
            db.Entry(subscriber).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
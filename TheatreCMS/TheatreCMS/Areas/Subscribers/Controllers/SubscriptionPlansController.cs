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

namespace TheatreCMS.Areas.Subscribers.Controllers
{
    public class SubscriptionPlansController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Subscribers/SubscriptionPlans
        public ActionResult Index()
        {
            return View(db.SubscriptionPlan.ToList());
        }

        // GET: Subscribers/SubscriptionPlans/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SubscriptionPlan subscriptionPlan = db.SubscriptionPlan.Find(id);
            if (subscriptionPlan == null)
            {
                return HttpNotFound();
            }
            return View(subscriptionPlan);
        }

        // GET: Subscribers/SubscriptionPlans/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Subscribers/SubscriptionPlans/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SubscriptionLevel,PricePerYear,NumberOfShows")] SubscriptionPlan subscriptionPlan)
        {
            ComparePlanOptions(subscriptionPlan);
            if (ModelState.IsValid)
            {
                db.SubscriptionPlan.Add(subscriptionPlan);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(subscriptionPlan);
        }

        // GET: Subscribers/SubscriptionPlans/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SubscriptionPlan subscriptionPlan = db.SubscriptionPlan.Find(id);
            if (subscriptionPlan == null)
            {
                return HttpNotFound();
            }
            return View(subscriptionPlan);
        }

        // POST: Subscribers/SubscriptionPlans/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PlanId,SubscriptionLevel,PricePerYear,NumberOfShows")] SubscriptionPlan subscriptionPlan)
        {
            CompareEditedPlans(subscriptionPlan);

            if (ModelState.IsValid)
            {
                db.Entry(subscriptionPlan).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(subscriptionPlan);
        }

        // GET: Subscribers/SubscriptionPlans/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SubscriptionPlan subscriptionPlan = db.SubscriptionPlan.Find(id);
            if (subscriptionPlan == null)
            {
                return HttpNotFound();
            }
            return View(subscriptionPlan);
        }

        // POST: Subscribers/SubscriptionPlans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SubscriptionPlan subscriptionPlan = db.SubscriptionPlan.Find(id);
            db.SubscriptionPlan.Remove(subscriptionPlan);
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

        public void CompareEditedPlans(SubscriptionPlan subscription)
        {
            //Assigns already existing plan with similar vaules to a var to exclude foreach loop
            var CurrentLevel = db.SubscriptionPlan.AsNoTracking().Where(p =>
                p.SubscriptionLevel == subscription.SubscriptionLevel ||
                p.PricePerYear == subscription.PricePerYear ||
                p.NumberOfShows == subscription.NumberOfShows).FirstOrDefault();
            //Check if current plan is null, if false program continues unhindered. If true enters foreach
            if (CurrentLevel != null)
            {
                //Assignment of dB excluding CurrentLevel to Var
                var SubscriptionPlanddB = db.SubscriptionPlan.AsNoTracking().ToList();
                SubscriptionPlanddB.RemoveAll(cl => cl.PlanId.Equals(CurrentLevel.PlanId));

                foreach (var plan in SubscriptionPlanddB)
                {
                    //Assigns both the users input and the objects specified attribute to strings
                    string SubscriptionLevel = subscription.SubscriptionLevel;
                    string ExistingSubscriptionLevel = plan.SubscriptionLevel;

                    //Assigns both the users input and the objects specified attribute to strings
                    decimal PricePerYear = subscription.PricePerYear;
                    decimal ExistingPricePerYear = plan.PricePerYear;

                    //Assigns both the users input and the objects specified attribute to strings
                    int NumberOfShows = subscription.NumberOfShows;
                    int ExistingNumberOfShows = plan.NumberOfShows;

                    //comparason of users input and specified attribute, check for equal
                    if (ExistingSubscriptionLevel.Equals(SubscriptionLevel))
                    {
                        //if equal then trigger model error to display on html page
                        ModelState.AddModelError("SubscriptionPlan", " There is already a plan with that Subscription Level named: " + SubscriptionLevel);
                    }
                    //comparason of users input and specified attribute, check for equal
                    if (ExistingPricePerYear.Equals(PricePerYear))
                    {
                        //if equal then trigger model error to display on html page
                        ModelState.AddModelError("SubscriptionPlan", " There is already a plan that offers a price/year of: " + PricePerYear);
                    }
                    //comparason of users input and specified attribute, check for equal
                    if (ExistingNumberOfShows.Equals(NumberOfShows))
                    {
                        //if equal then trigger model error to display on html page
                        ModelState.AddModelError("SubscriptionPlan", " There is already a Plan that offers:  " + NumberOfShows);
                    }
                }
            }
        }



            public void ComparePlanOptions(SubscriptionPlan subscriptionPlan)
        {
            //Checks each object in dB 
            foreach (var plan in db.SubscriptionPlan)
            {
                //Assigns both the users input and the objects specified attribute to strings
                string SubscriptionLevel = subscriptionPlan.SubscriptionLevel;
                string ExistingSubscriptionLevel = plan.SubscriptionLevel;

                //Assigns both the users input and the objects specified attribute to strings
                decimal PricePerYear = subscriptionPlan.PricePerYear;
                decimal ExistingPricePerYear = plan.PricePerYear;

                //Assigns both the users input and the objects specified attribute to strings
                int NumberOfShows = subscriptionPlan.NumberOfShows;
                int ExistingNumberOfShows = plan.NumberOfShows;

                //comparason of users input and specified attribute, check for equal
                if (ExistingSubscriptionLevel.Equals(SubscriptionLevel))
                {
                    //if equal then trigger model error to display on html page
                    ModelState.AddModelError("SubscriptionPlan", " There is already a plan with that Subscription Level named: " + SubscriptionLevel);
                }
                //comparason of users input and specified attribute, check for equal
                if (ExistingPricePerYear.Equals(PricePerYear))
                {
                    //if equal then trigger model error to display on html page
                    ModelState.AddModelError("SubscriptionPlan", " There is already a plan that offers a price/year of: " + PricePerYear);
                }
                //comparason of users input and specified attribute, check for equal
                if (ExistingNumberOfShows.Equals(NumberOfShows))
                {
                    //if equal then trigger model error to display on html page
                    ModelState.AddModelError("SubscriptionPlan", " There is already a Plan that offers:  " + NumberOfShows);
                }
            }

        }
    }
}

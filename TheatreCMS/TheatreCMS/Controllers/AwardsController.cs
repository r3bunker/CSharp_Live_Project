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
    public class AwardsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Awards
        public ActionResult Index()
        {
            var awards = db.Awards.Include(a => a.CastMember).Include(a => a.Production);
            return View(awards.ToList());
        }

        // GET: Awards/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Award award = db.Awards.Find(id);
            if (award == null)
            {
                return HttpNotFound();
            }
            return View(award);
        }


        //The two below methods are not needed when not using AJAX on the create page

        //Gets production name from dB and returns it
        //public string GetProductionName(int? productionId)
        //{
        //    if (productionId != null)
        //    {
        //        Production production = db.Productions.Find(productionId);
        //        return production.Title;
        //    }
        //    return "";    
        //}

        //public string GetAwardType(int? awardId)
        //{
        //    if (awardId != null)
        //    {
        //        return ((AwardType)awardId).ToString();
        //    }
        //    return "";
        //}

        // GET: Awards/Create
        public ActionResult Create()
        {
            ViewBag.CastMemberId = new SelectList(db.CastMembers, "CastMemberID", "Name");
            ViewBag.ProductionId = new SelectList(db.Productions, "ProductionId", "Title");

            // Added Production Years to change Award Year to reflect changing Production 
            ViewBag.ProductionYears = db.Productions.Select(x => x.OpeningDay.Year).ToList();

            // Using list of CastMembers to validate Recipient connection.  
            ViewBag.CastMembers = db.CastMembers.Select(x => x.Name).ToList();
            
            //int yeardiff = DateTime.Now.Year - 1997 + 2;        //presents range of years as dropdown
            // ViewBag.Year = new SelectList(Enumerable.Range(1997, yeardiff));  // Do not need this in controller. 

            //ViewBag.Type = new SelectList("Award", "Finalist", "Other");
            
            return View();
        }

       
    // POST: Awards/Create
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AwardId,Year,Name,Type,Category,Recipient,ProductionId,CastMemberId,OtherInfo")] Award award)
        {
            if (ModelState.IsValid)
            {
                db.Awards.Add(award);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CastMemberId = new SelectList(db.CastMembers, "CastMemberID", "Name", award.CastMemberId);
            ViewBag.ProductionId = new SelectList(db.Productions, "ProductionId", "Title", award.ProductionId);
            
            return View(award);
        }

        // GET: Awards/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Award award = db.Awards.Find(id);
            if (award == null)
            {
                return HttpNotFound();
            }
            ViewBag.CastMemberId = new SelectList(db.CastMembers, "CastMemberID", "Name", award.CastMemberId);
            ViewBag.ProductionId = new SelectList(db.Productions, "ProductionId", "Title", award.ProductionId);
            
            // Added Production Years to change Award Year to reflect changing Production 
            ViewBag.ProductionYears = db.Productions.Select(x => x.OpeningDay.Year).ToList();

            // Using list of CastMembers to validate Recipient connection.  
            ViewBag.CastMembers = db.CastMembers.Select(x => x.Name).ToList();
            return View(award);
        }

        // POST: Awards/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AwardId,Year,Name,Type,Category,Recipient,ProductionId,CastMemberId,OtherInfo")] Award award)
        {
            if (ModelState.IsValid)
            {
                db.Entry(award).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CastMemberId = new SelectList(db.CastMembers, "CastMemberID", "Name", award.CastMemberId);
            ViewBag.ProductionId = new SelectList(db.Productions, "ProductionId", "Title", award.ProductionId);
            return View(award);
        }

        // GET: Awards/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Award award = db.Awards.Find(id);
            if (award == null)
            {
                return HttpNotFound();
            }
            return View(award);
        }

        // POST: Awards/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Award award = db.Awards.Find(id);
            db.Awards.Remove(award);
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

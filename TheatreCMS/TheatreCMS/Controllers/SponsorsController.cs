using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TheatreCMS.Models;
using TheatreCMS.Controllers;
using System.IO;

namespace TheatreCMS.Controllers
{
    
    public class SponsorsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [Authorize(Roles = "Admin")]
        // GET: Sponsors
        public ActionResult Index()
        {          
            return View(db.Sponsors.ToList());
        }

        ////List of sponsors, partial view _Sponsors
        //public ActionResult List()
        //{
        //    return View("_Sponsors", db.Sponsors); 
        //}


        //Method for partial view 
        public ActionResult List()
        {
            return PartialView("_SponsorsPartial", db.Sponsors);
        }

        [Authorize(Roles = "Admin")]
        // GET: Sponsors/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sponsor sponsor = db.Sponsors.Find(id);
            if (sponsor == null)
            {
                return HttpNotFound();
            }
            return View(sponsor);
        }

        [Authorize(Roles = "Admin")]
        // GET: Sponsors/Create
        public ActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        // POST: Sponsors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SponsorId,Name,Height,Width,Current,Link")] Sponsor sponsor, HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength > 0)
                {
                    //takes upload file and creates a photo.cs object from it and returns (int)photo.ID  
                    sponsor.PhotoId = PhotoController.CreatePhoto(upload, "SponsorLogo_" + sponsor.Name);
                }
                db.Sponsors.Add(sponsor);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(sponsor);

        }

        [Authorize(Roles = "Admin")]
        // GET: Sponsors/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sponsor sponsor = db.Sponsors.Find(id);
            if (sponsor == null)
            {
                return HttpNotFound();
            }
            return View(sponsor);
        }

        [Authorize(Roles = "Admin")]
        // POST: Sponsors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SponsorId,Name,PhotoId,Height,Width,Current,Link")] Sponsor sponsor, HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength > 0)
                {
                    sponsor.PhotoId = PhotoController.CreatePhoto(upload, "SponsorLogo_" + sponsor.Name);
                }

                db.Entry(sponsor).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(sponsor);
        }

        [Authorize(Roles = "Admin")]
        // GET: Sponsors/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sponsor sponsor = db.Sponsors.Find(id);
            if (sponsor == null)
            {
                return HttpNotFound();
            }
            return View(sponsor);
        }

        [Authorize(Roles = "Admin")]
        // POST: Sponsors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Sponsor sponsor = db.Sponsors.Find(id);
            db.Sponsors.Remove(sponsor);
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

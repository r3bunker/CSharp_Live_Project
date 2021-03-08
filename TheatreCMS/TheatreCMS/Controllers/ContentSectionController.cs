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
    [Authorize(Roles = "Admin")]
    public class ContentSectionController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ContentSection
        public ActionResult Index()
        {
            return View(db.ContentSections.ToList());
        }

        // GET: ContentSection/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContentSection contentSection = db.ContentSections.Find(id);
            if (contentSection == null)
            {
                return HttpNotFound();
            }
            return View(contentSection);
        }

        // GET: ContentSection/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ContentSection/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SectionId,ContentType,ContentId,CssId")] ContentSection contentSection)
        {
            if (ModelState.IsValid)
            {
                db.ContentSections.Add(contentSection);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(contentSection);
        }

        // GET: ContentSection/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContentSection contentSection = db.ContentSections.Find(id);
            if (contentSection == null)
            {
                return HttpNotFound();
            }
            return View(contentSection);
        }

        // POST: ContentSection/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SectionId,ContentType,ContentId,CssId")] ContentSection contentSection)
        {
            if (ModelState.IsValid)
            {
                db.Entry(contentSection).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(contentSection);
        }

        // GET: ContentSection/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContentSection contentSection = db.ContentSections.Find(id);
            if (contentSection == null)
            {
                return HttpNotFound();
            }
            return View(contentSection);
        }

        // POST: ContentSection/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ContentSection contentSection = db.ContentSections.Find(id);
            db.ContentSections.Remove(contentSection);
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

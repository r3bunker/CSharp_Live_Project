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
    public class DisplayLinksController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: DisplayLinks
        public ActionResult Index()
        {
            return View(db.DisplayLinks.OrderBy(s=>s.Name).ToList());
        }

        // GET: DisplayLinks/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DisplayLinks displayLinks = db.DisplayLinks.Find(id);
            if (displayLinks == null)
            {
                return HttpNotFound();
            }
            return View(displayLinks);
        }

        // GET: DisplayLinks/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DisplayLinks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LinkId,Name,Text,Link")] DisplayLinks displayLinks)
        {
            if (ModelState.IsValid)
            {
                db.DisplayLinks.Add(displayLinks);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(displayLinks);
        }

        // GET: DisplayLinks/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DisplayLinks displayLinks = db.DisplayLinks.Find(id);
            if (displayLinks == null)
            {
                return HttpNotFound();
            }
            return View(displayLinks);
        }

        // POST: DisplayLinks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "LinkId,Name,Text,Link")] DisplayLinks displayLinks)
        {
            if (ModelState.IsValid)
            {
                db.Entry(displayLinks).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(displayLinks);
        }

        // GET: DisplayLinks/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DisplayLinks displayLinks = db.DisplayLinks.Find(id);
            if (displayLinks == null)
            {
                return HttpNotFound();
            }
            return View(displayLinks);
        }

        // POST: DisplayLinks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DisplayLinks displayLinks = db.DisplayLinks.Find(id);
            db.DisplayLinks.Remove(displayLinks);
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

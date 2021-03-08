using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TheatreCMS.Models;
using System.Diagnostics;

namespace TheatreCMS.Controllers
{
    public class NewsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: list of Published News
        public ActionResult Published()
        {
            if (User.IsInRole("Admin"))
            {
                return View(db.News.OrderByDescending(i => i.LastSaveDate).ToList());
            }
            else
            {
                return View(db.News.Where(i => i.PublishDate != null && i.Hidden == false).OrderByDescending(i => i.PublishDate).ToList());
            }
        }

        [Authorize(Roles = "Admin")]
        // GET: News
        public ActionResult Index()
        {
            return View(db.News.ToList());
        }

        [Authorize(Roles = "Admin")]
        // GET: News/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            News news = db.News.Find(id);
            if (news == null)
            {
                return HttpNotFound();
            }
            news.Headline = HttpUtility.HtmlDecode(news.Headline);
            news.Content = HttpUtility.HtmlDecode(news.Content);
            return View(news);
        }

        [Authorize(Roles = "Admin")]
        // GET: News/Create
        public ActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        // POST: News/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "NewsId,Headline,Content,PublishDate,EmailDate,Hidden")] News news)
        {
            if (ModelState.IsValid)
            {
                news.Headline = HttpUtility.HtmlEncode(news.Headline);
                news.Content = HttpUtility.HtmlEncode(news.Content);
                news.CreateDate = DateTime.Now;
                news.LastSaveDate = DateTime.Now;
                db.News.Add(news);
                db.SaveChanges();
                return RedirectToAction("Published");
            }

            return View(news);
        }

        [Authorize(Roles = "Admin")]
        // GET: News/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            News news = db.News.Find(id);
            if (news == null)
            {
                return HttpNotFound();
            }
            news.Headline = HttpUtility.HtmlDecode(news.Headline);
            news.Content = HttpUtility.HtmlDecode(news.Content);
            return View(news);
        }

        [Authorize(Roles = "Admin")]
        // POST: News/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "NewsId,Headline,Content,CreateDate,LastSaveDate,PublishDate,EmailDate,Hidden")] News news)
        {
            if (ModelState.IsValid)
            {
                news.Headline = HttpUtility.HtmlEncode(news.Headline);
                news.Content = HttpUtility.HtmlEncode(news.Content);
                db.Entry(news).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(news);
        }

        [Authorize(Roles = "Admin")]
        // GET: News/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            News news = db.News.Find(id);
            if (news == null)
            {
                return HttpNotFound();
            }
            news.Headline = HttpUtility.HtmlDecode(news.Headline);
            news.Content = HttpUtility.HtmlDecode(news.Content);
            return View(news);
        }

        [Authorize(Roles = "Admin")]
        // POST: News/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            News news = db.News.Find(id);
            db.News.Remove(news);
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

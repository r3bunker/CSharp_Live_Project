using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TheatreCMS.Models;
using System.Drawing;
using TheatreCMS.Controllers;

namespace TheatreCMS.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DisplayInfoController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: DisplayInfo
        public ActionResult Index()
        {
            return View(db.DisplayInfo.ToList());
        }

        // GET: DisplayInfo/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DisplayInfo displayInfo = db.DisplayInfo.Find(id);
            if (displayInfo == null)
            {
                return HttpNotFound();
            }
            return View(displayInfo);
        }

        // GET: DisplayInfo/Create
        public ActionResult Create()
        {

            return View();
        }

        // POST: DisplayInfo/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        //The ModelState variable only holds three key/value pairs in its dictionary after POST.
        //They are Title, Description and File (which is the file name, and should so be called.
        //Not sure whether or not Image should be in the Include parameter of the Bind method...

        public ActionResult Create([Bind(Include = "InfoId,Title,TextContent,File")] DisplayInfo displayInfo, HttpPostedFileBase file)
        {
            //byte[] image = ImageUploadController.ImageBytes(file, out string _64);
            //displayInfo.Image = image;
            //displayInfo.File = file.FileName;

            if (ModelState.IsValid)
            {
                if (file != null && file.ContentLength > 0)
                {
                    //takes upload file and creates a photo.cs object from it and returns (int)photo.ID  
                    displayInfo.PhotoId = PhotoController.CreatePhoto(file, displayInfo.Title);
                }
                db.DisplayInfo.Add(displayInfo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(displayInfo);
        }

        // GET: DisplayInfo/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DisplayInfo displayInfo = db.DisplayInfo.Find(id);
            if (displayInfo == null)
            {
                return HttpNotFound();
            }
            return View(displayInfo);
        }

        // POST: DisplayInfo/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "InfoId,Title,TextContent,Image, File")] DisplayInfo displayInfo, HttpPostedFileBase file)
        {
            //validates photo
            if(file != null && !PhotoController.ValidatePhoto(file))
            {
                ModelState.AddModelError("Photo", "File must be a valid photo format.");
            }
            
            if (ModelState.IsValid)
            {
                if (file != null && file.ContentLength > 0)
                {
          //updates the new displayInfo's photo, and textcontent
                    displayInfo.PhotoId = PhotoController.CreatePhoto(file, "DisplayInfoPhoto_" + displayInfo.Title);
                }
                db.Entry(displayInfo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(displayInfo);
        }
         
        // GET: DisplayInfo/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DisplayInfo displayInfo = db.DisplayInfo.Find(id);
            if (displayInfo == null)
            {
                return HttpNotFound();
            }
            return View(displayInfo);
        }

        // POST: DisplayInfo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DisplayInfo displayInfo = db.DisplayInfo.Find(id);
            db.DisplayInfo.Remove(displayInfo);
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

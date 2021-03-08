using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Windows.Media.Animation;
using TheatreCMS.Models;
using TheatreCMS.ViewModels;

namespace TheatreCMS.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PhotoController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Photo
        public ActionResult Index()
        {
            return View(db.Photo.ToList());
        }

        // Action method for displaying infinite scroll 
        public ActionResult GetPhotos(int pageIndex, int pageSize)
        {
            System.Threading.Thread.Sleep(500);  //sets a delay on loading. Used for debugging.
            var query = (from photo in db.Photo
                         orderby photo.PhotoId ascending
                         select new { photo.PhotoId, photo.OriginalHeight, photo.OriginalWidth, photo.Title }).Skip(pageIndex * pageSize).Take(pageSize);  // selecting anonymous type is done to prevent passing the byte array in the PhotFile attribute 

            return Json(Newtonsoft.Json.JsonConvert.SerializeObject(query), JsonRequestBehavior.AllowGet);
        }

        // GET: Photo/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Photo photo = db.Photo.Find(id);
            if (photo == null)
            {
                return HttpNotFound();
            }
            return View(photo);
        }

        //file -> byte[]
        [AllowAnonymous]
        public static byte[] ImageBytes(HttpPostedFileBase file)
        {
            //Convert the file into a System.Drawing.Image type
            Image image = Image.FromStream(file.InputStream, true, true);
            //Convert that image into a Byte Array to facilitate storing the image in a database
            var converter = new ImageConverter();
            byte[] imageBytes = (byte[])converter.ConvertTo(image, typeof(byte[]));
            //return Byte Array
            return imageBytes;
        }

        //byte[] -> smaller byte[]
        [AllowAnonymous]
        public static byte[] ImageThumbnail(byte[] imageBytes, int thumbWidth, int thumbHeight)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Image img = Image.FromStream(new MemoryStream(imageBytes));
                using (Image thumbnail = img.GetThumbnailImage(img.Width, img.Height, null, new IntPtr()))
                {
                    thumbnail.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    return ms.ToArray();
                }
            }
        }

        // GET: Photo/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Photo/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PhotoId,PhotoFile,OriginalHeight,OriginalWidth,Title")] Photo photo, HttpPostedFileBase file)
        {
            byte[] photoArray = ImageBytes(file);
            if (db.Photo.Where(x => x.PhotoFile == photoArray).ToList().Any())
            {
                var id = db.Photo.Where(x => x.PhotoFile == photoArray).ToList().FirstOrDefault().PhotoId;
                ModelState.AddModelError("PhotoFile", "This photo already exists in the database. Would you like to <a href='/Photo/Details/" + id + "'>view</a> or <a href='/Photo/Edit/" + id + "'>edit</a> the photo?");
            }
            if (photo.Title == null)
            {
                ModelState.AddModelError("Title", "The photo must have a title");
            }
            if (ModelState.IsValid)
            {
                photo.PhotoFile = photoArray;
                Bitmap img = new Bitmap(file.InputStream);
                photo.OriginalHeight = img.Height;
                photo.OriginalWidth = img.Width;

                db.Photo.Add(photo);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(photo);
        }

        [AllowAnonymous]
        public static int CreatePhoto(HttpPostedFileBase file, string title)

        {
            var photo = new Photo();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                byte[] photoArray = ImageBytes(file);
                if (db.Photo.Where(x => x.PhotoFile == photoArray).ToList().Any())
                {
                    var id = db.Photo.Where(x => x.PhotoFile == photoArray).ToList().FirstOrDefault().PhotoId;
                    return id;
                }
                else
                {
                    photo.Title = title;
                    Image image = Image.FromStream(file.InputStream, true, true);
                    photo.OriginalHeight = image.Height;
                    photo.OriginalWidth = image.Width;
                    var converter = new ImageConverter();
                    photo.PhotoFile = (byte[])converter.ConvertTo(image, typeof(byte[]));
                    db.Photo.Add(photo);
                    db.SaveChanges();
                    return photo.PhotoId;
                }
            }
        }

        //========== VALIDATE PHOTO - Test if input is photo, return True if a photo format
        public static bool ValidatePhoto(HttpPostedFileBase postedFile)
        {
            Debug.WriteLine("Validating photo...");
            const int ImageMinimumBytes = 512;
            //-------------------------------------------
            //  Check the image mime types
            //-------------------------------------------
            if (!string.Equals(postedFile.ContentType, "image/jpg", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(postedFile.ContentType, "image/jpeg", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(postedFile.ContentType, "image/pjpeg", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(postedFile.ContentType, "image/gif", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(postedFile.ContentType, "image/x-png", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(postedFile.ContentType, "image/png", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            //-------------------------------------------
            //  Check the image extension
            //-------------------------------------------
            var postedFileExtension = Path.GetExtension(postedFile.FileName);
            if (!string.Equals(postedFileExtension, ".jpg", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(postedFileExtension, ".png", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(postedFileExtension, ".gif", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(postedFileExtension, ".jpeg", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            //-------------------------------------------
            //  Attempt to read the file and check the first bytes
            //-------------------------------------------
            try
            {
                if (!postedFile.InputStream.CanRead)
                {
                    return false;
                }
                //----- Check whether the image size below the lower limit or not
                if (postedFile.ContentLength < ImageMinimumBytes)
                {
                    return false;
                }
                //----- Read the file
                byte[] buffer = new byte[ImageMinimumBytes];
                postedFile.InputStream.Read(buffer, 0, ImageMinimumBytes);
                string content = System.Text.Encoding.UTF8.GetString(buffer);
                if (Regex.IsMatch(content, @"<script|<html|<head|<title|<body|<pre|<table|<a\s+href|<img|<plaintext|<cross\-domain\-policy",
                    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline))
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
            //-------------------------------------------
            //  Try to instantiate new Bitmap, if .NET will throw exception
            //  we can assume that it's not a valid image
            //-------------------------------------------
            try
            {
                using (var bitmap = new Bitmap(postedFile.InputStream))
                {
                }
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                postedFile.InputStream.Position = 0;
            }
            return true;
        }


        //Takes an image file and title string and returns the PhotoId as string
        [HttpPost]
        [ValidateAntiForgeryToken]
        public string GetPhotoId(HttpPostedFileBase file, string title)
        {
            if (ModelState.IsValid)
            {
                var photo = new Photo();
                photo.Title = title;
                Image image = Image.FromStream(file.InputStream, true, true);
                photo.OriginalHeight = image.Height;
                photo.OriginalWidth = image.Width;
                var converter = new ImageConverter();
                photo.PhotoFile = (byte[])converter.ConvertTo(image, typeof(byte[]));
                db.Photo.Add(photo);
                db.SaveChanges();
                return (photo.PhotoId).ToString();
    
            }

            return null;
        }


        [AllowAnonymous]
        public ActionResult DisplayPhoto(int? id) //nullable int
        {
            string filePath = Server.MapPath(Url.Content("~/Content/Images/no-image.png"));
            Image noImageAvail = Image.FromFile(filePath);
            var converter = new ImageConverter();
            var byteData = (byte[])converter.ConvertTo(noImageAvail, typeof(byte[]));
            if (id.HasValue)
            {
                Photo photo = db.Photo.Find(id);
                if (photo == null)
                {
                    return File(byteData, "image/png");
                }
                else
                {
                    return File(photo.PhotoFile, "image/png");
                }
            }
            else
            {
                return File(byteData, "image/png");
            }
        }

        // GET: Photo/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Photo photo = db.Photo.Find(id);
            if (photo == null)
            {
                return HttpNotFound();
            }
            return View(photo);
        }

        // POST: Photo/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PhotoId,PhotoFile,OriginalHeight,OriginalWidth,Title")] Photo photo, HttpPostedFileBase file)
        {
            var currentphoto = db.Photo.Find(photo.PhotoId);
            currentphoto.Title = photo.Title;
            
            if (currentphoto.Title == null)
            {
                ModelState.AddModelError("Title", "The photo must have a title");
                return View(currentphoto);
            }

            if (file == null)
            {
                db.Entry(currentphoto).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                byte[] photoArray = ImageBytes(file);
                if (db.Photo.Where(x => x.PhotoFile == photoArray).ToList().Any())
                {
                    var id = db.Photo.Where(x => x.PhotoFile == photoArray).ToList().FirstOrDefault().PhotoId;
                    ModelState.AddModelError("PhotoFile", "This photo already exists in the database. Would you like to <a href='/Photo/Details/" + id + "'>view</a> the photo?");
                }

                if (ModelState.IsValid)
                {
                    currentphoto.PhotoFile = photoArray;
                    Bitmap img = new Bitmap(file.InputStream);
                    currentphoto.OriginalHeight = img.Height;
                    currentphoto.OriginalWidth = img.Width;
                    db.Entry(currentphoto).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }   
            }
            return View(currentphoto);
        }
        
        // GET: Photo/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Photo photo = db.Photo.Find(id);
            if (photo == null)
            {
                return HttpNotFound();
            }
            return View(photo);
        }

        // POST: Photo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                Photo photo = db.Photo.Find(id);

                ProductionPhotos productionPhoto = db.ProductionPhotos.FirstOrDefault(x => x.PhotoId == photo.PhotoId);
                if (productionPhoto != null)
                {
                    DbEntityEntry<ProductionPhotos> dbEntityEntry = db.Entry(productionPhoto);
                    ProductionPhotos photoUnavailable = db.ProductionPhotos.Where(b => b.Title == "Photo Unavailable").FirstOrDefault();
                    productionPhoto.PhotoId = photoUnavailable.PhotoId;
                    dbEntityEntry.CurrentValues.SetValues(productionPhoto);
                }
                // Checks for a production using this photo as a deleted photo
                Production production = db.Productions.FirstOrDefault(x => x.DefaultPhoto.PhotoId == photo.PhotoId);
                if (production != null)  // If Production exists
                {
                    // Checks to see if there is another photo related to the production
                    if (production.ProductionPhotos.Where(p => p.PhotoId != null).Count() > 0)
                    {
                        foreach (var potentialDefaultPhoto in production.ProductionPhotos)
                        {
                            if (potentialDefaultPhoto.PhotoId == photo.PhotoId) continue;  // Ignores current default photo
                            // Error handling for deleted photos that still have references
                            else if (potentialDefaultPhoto == null || potentialDefaultPhoto.PhotoId == null) continue;
                            else
                            {
                                // Sets new default photo
                                production.DefaultPhoto = potentialDefaultPhoto;
                                break;  // Exists the loop since a photo has been found
                            }
                        }
                    }
                    // Sets the default photo to "Photo Unavailable"
                    else production.DefaultPhoto = db.ProductionPhotos.Where(p => p.Title == "Photo Unavailable").FirstOrDefault();
                    DbEntityEntry<Production> dbEntityEntry = db.Entry(production);
                    dbEntityEntry.CurrentValues.SetValues(production);
                }
                Sponsor sponsor = db.Sponsors.FirstOrDefault(x => x.PhotoId == photo.PhotoId);
                if (sponsor != null)
                {
                    DbEntityEntry<Sponsor> dbEntityEntry = db.Entry(sponsor);
                    sponsor.PhotoId = null;
                    dbEntityEntry.CurrentValues.SetValues(sponsor);
                }
                CastMember cast = db.CastMembers.FirstOrDefault(x => x.PhotoId == photo.PhotoId);
                if (cast != null)
                {
                    DbEntityEntry<CastMember> dbEntityEntry = db.Entry(cast);
                    cast.PhotoId = null;
                    dbEntityEntry.CurrentValues.SetValues(cast);
                }
                db.Photo.Remove(photo);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }


        [AllowAnonymous]
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        /// <summary>
        /// Find the dependencies for a photo and to allow them to be displayed on the details and edit views.  
        /// </summary>
        /// <param name="Id">The Id of the photo that can be used to find the dependencies.</param>
        /// <returns>Photo Dependencies View Model</returns>
        public static PhotoDependenciesVm FindDependencies(int? Id)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var photoDependencies = new PhotoDependenciesVm();
                photoDependencies.ValidId = true;
                if (Id == null)                                                                                              //If the Id argument is null, ValidId is returned false and the method stops
                {
                    photoDependencies.ValidId = false;
                    return null;                                                                                             //Will cause issues if the Id argument is invalid
                }
                var photoEntity = db.Photo.Find(Id);                                                                         //Declaring photo object from passed argument Id
                var sponsorEntity = db.Sponsors.FirstOrDefault(photo => photo.PhotoId == photoEntity.PhotoId);                //Declaring sponsor object from photo object
                if (sponsorEntity != null && sponsorEntity.PhotoId == photoEntity.PhotoId)                                    //Checks if there is a sponsor, and if that sponsor's photo id matches
                {
                    photoDependencies.Sponsors.Add(sponsorEntity);                                                           //Adds sponsor object to sponsors list inside ViewModel
                }
                var productionPhotoEntity = db.ProductionPhotos.FirstOrDefault(photo => photo.PhotoId == photoEntity.PhotoId);    //Declaring production object from photo object
                if (productionPhotoEntity != null && productionPhotoEntity.PhotoId == photoEntity.PhotoId)                             //Checks if there is a production, and if the prod photo id matches
                {
                    photoDependencies.ProductionPhotos.Add(productionPhotoEntity);                                                //Adds prod object to production list inside ViewModel
                }
                var castMemberEntity = db.CastMembers.FirstOrDefault(Photo => Photo.PhotoId == photoEntity.PhotoId);         //Declaring a cast member object using passed in Id.
                if (castMemberEntity != null && castMemberEntity.PhotoId == photoEntity.PhotoId)                             //Check if there is a cast member if the Id finds a match
                {
                    photoDependencies.CastMembers.Add(castMemberEntity);                                                     //Add cast member object to list in the View Model
                }
                //Final check for dependencies. If either sponsorEntity or productionEntity are null an error is thrown, so an evaluation is necessary before comparing photo id's
                if (sponsorEntity != null && photoEntity.PhotoId == sponsorEntity.PhotoId || productionPhotoEntity != null && photoEntity.PhotoId == productionPhotoEntity.PhotoId || castMemberEntity != null && photoEntity.PhotoId == castMemberEntity.PhotoId)
                {
                    photoDependencies.HasDependencies = true;
                }
                int season;
                if (productionPhotoEntity != null) season = productionPhotoEntity.Production.Season;                                   //Gets the producton's season before closing the connection to the database
                return photoDependencies;
            }
            
        }
    }
}

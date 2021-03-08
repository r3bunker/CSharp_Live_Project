using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;   // For testing purposes
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using TheatreCMS.Models;
//using Microsoft.AspNet.Identity;

namespace TheatreCMS.Controllers
{
    public class CastMembersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: CastMembers
        public ActionResult Index()
        {
            //Creates a dictionary of Id's and Usernames passing it to the View 
             var Users = from a in db.Users select new { a.Id, a.UserName };
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
            foreach (var user in Users)
                keyValuePairs.Add(user.Id, user.UserName);
            ViewBag.Users = keyValuePairs;

            if (Request.IsAuthenticated)
            {
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                ApplicationUser currentUser = userManager.FindById(User.Identity.GetUserId());

                if (currentUser != null && currentUser.FavoriteCastMembers != null)
                {
                    // Break the string down into a list and send to view
                    ViewBag.FavCastIds = currentUser.FavoriteCastMembers.Split(',').ToList();
                }

                // Find cast member id
                if (currentUser.Role == "Member")
                {
                    ViewBag.memberId = currentUser.CastMemberUserID;
                }
            }
            var list = db.CastMembers.ToList();
            return View(list);
        }

        

        // GET: CastMembers/Details/5
        public ActionResult Details(int? id)
        {
            ViewData["dbUsers"] = new SelectList(db.Users.ToList(), "Id", "UserName");
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CastMember castMember = db.CastMembers.Find(id);
            if (castMember == null)
            {
                return HttpNotFound();
            }
            //Passes The Username of the currently selected cast member to the model
            if (castMember.CastMemberPersonID != null)
                ViewBag.CurrentUser = db.Users.Find(castMember.CastMemberPersonID).UserName;

            if (Request.IsAuthenticated)
            {
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                ApplicationUser currentUser = userManager.FindById(User.Identity.GetUserId());

                if (currentUser != null && currentUser.FavoriteCastMembers != null)
                {
                    // Break the string down into a list and send to view
                    ViewBag.FavCastIds = currentUser.FavoriteCastMembers.Split(',').ToList();
                }
            }

            return View(castMember);
        }

        // GET: CastMembers/Create
        [TheatreAuthorize(Roles = "Admin")]
        public ActionResult Create()
        {
            ViewData["dbUsers"] = new SelectList(db.Users.ToList(), "Id", "UserName");
            
            return View();
        }

        // POST: CastMembers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CastMemberID,Name,YearJoined,MainRole,Bio,PhotoId,CurrentMember,CastMemberPersonId,AssociateArtist,EnsembleMember,CastYearLeft,DebutYear")] CastMember castMember, HttpPostedFileBase file)
        {
         
            ModelState.Remove("CastMemberPersonID");

            //Extract the Guid as type String from user's selected User (from SelectList)
            string userId = Request.Form["dbUsers"].ToString();

            // ModelState error to ensure that A user cannot be assigned to multiple cast members.
            // If the CastMemberUserID IS assigned for this user, that means that this user is assigned
            // to another Cast Member: add the ModelState error.
            if (!string.IsNullOrEmpty(userId) && db.Users.Find(userId).CastMemberUserID != 0)
                ModelState.AddModelError("CastMemberPersonID", $"{db.Users.Find(userId).UserName} already has a cast member profile");

            if (ModelState.IsValid)
            {
                if (file != null && file.ContentLength > 0)
                {
                              
                    castMember.PhotoId = PhotoController.CreatePhoto(file, castMember.Name); // Call CreatePhoto method from Photocontroller and assign return value (int photo.PhotoId) to castMember.PhotoId
                }
               

                //ViewData["dbUsers"] = new SelectList(db.Users.ToList(), "Id", "UserName");

                if (!string.IsNullOrEmpty(userId))
                {
                    castMember.CastMemberPersonID = db.Users.Find(userId).Id;
                }
                //ModelState.Remove("PhotoId");
                db.CastMembers.Add(castMember);
                db.SaveChanges();

                // If a user was selected, update the CastMemberUserID column in the User table with CastMemberPersonID.
                if (!string.IsNullOrEmpty(userId))
                {
                    // Find the selected user.
                    var selectedUser = db.Users.Find(userId);

                    // Update the User's Cast Member Id column with castMemberId
                    selectedUser.CastMemberUserID = castMember.CastMemberID;

                    // Save the changes
                    db.Entry(selectedUser).State = EntityState.Modified;
                    db.SaveChanges();
                }

                return RedirectToAction("Index");
            }
            else  // This viewdata is required for the create view
            {
                ViewData["dbUsers"] = new SelectList(db.Users.ToList(), "Id", "UserName");
            }

            return View(castMember);
        }

        // GET: CastMembers/Edit/5
        // Only authorize access to users with the 'Admin' role
        [TheatreAuthorize(Roles = "Admin, Member")]
        public ActionResult Edit(int? id)
        {
            // STORY REQUIREMENT: The Edit function should check if the User has been modified(i.e. if the User has been added, 
            // removed, or changed) and set or reset the User(or Users) value for the appropriate CastMemberId.
           
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            CastMember castMember = db.CastMembers.Find(id);

            if (castMember == null)
            {
                return HttpNotFound();
            }

            // Check if the user associated with this CastMember is deleted.  If so, reset value to null.
            // If the user is already null, don't look for matching ids and don't update the database.
            if (castMember.CastMemberPersonID != null && db.Users.Where(x => x.Id == castMember.CastMemberPersonID).Count() <= 0)
            {
                Debug.WriteLine("\n\n\nDELETED USER DETECTED, Reset Username to N / A\n\n\n");
                castMember.CastMemberPersonID = null;

                db.Entry(castMember).State = EntityState.Modified;
                db.SaveChanges();
            }
            // ***still need to get existing value to display as a default in drop-down list***
            ViewData["dbUsers"] = new SelectList(db.Users.ToList(), "Id", "UserName", castMember.CastMemberPersonID);
            
            return View(castMember);
        }

        // POST: CastMembers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        // Only authorize access to users with the 'Admin' role
        [TheatreAuthorize(Roles = "Admin, Member")]
        public ActionResult Edit([Bind(Include = "CastMemberID,Name,YearJoined,MainRole,Bio,PhotoId,CurrentMember,AssociateArtist,EnsembleMember,CastYearLeft,DebutYear")] CastMember castMember, HttpPostedFileBase file)
        {
            ModelState.Remove("CastMemberPersonID");
            string userId = Request.Form["dbUsers"].ToString();

            // ModelState error to ensure that A user cannot be assigned to multiple cast members.
            // If the userId is null, castMemberId is 0, or previous castMemberId is the same as the new CastMemberId,
            // Then don't add the model error.
            if (!string.IsNullOrEmpty(userId))
            {
                int newCastMemberId = db.Users.Find(userId).CastMemberUserID;
                if (!(newCastMemberId == 0 || castMember.CastMemberID == newCastMemberId))
                    ModelState.AddModelError("CastMemberPersonID", $"{db.Users.Find(userId).UserName} already has a cast member profile");
            }

            // The unmodified Cast Member to be Edited ( The 'previous' Cast Member )
            var currentCastMember = db.CastMembers.Find(castMember.CastMemberID);

            if (ModelState.IsValid)
            {
                int? oldPhotoId = currentCastMember.PhotoId; // replace photo operation with photoid

                currentCastMember.Name = castMember.Name;
                currentCastMember.YearJoined = castMember.YearJoined;
                currentCastMember.MainRole = castMember.MainRole;
                currentCastMember.Bio = castMember.Bio;
                currentCastMember.CurrentMember = castMember.CurrentMember;
                currentCastMember.AssociateArtist = castMember.AssociateArtist;
                currentCastMember.EnsembleMember = castMember.EnsembleMember;
                currentCastMember.CastYearLeft = castMember.CastYearLeft;
                currentCastMember.DebutYear = castMember.DebutYear;

                string previousUserId = "";
                string newUserId = "";

                // If the Cast Member had a previous User, get that User's Id.
                if (!string.IsNullOrEmpty(currentCastMember.CastMemberPersonID))
                    previousUserId = currentCastMember.CastMemberPersonID;

                // If the selected UserName is not "(No User Selected)", get that User's Id.
                if (!string.IsNullOrEmpty(userId))
                    newUserId = userId;

                // Only change the Cast Member's and the user's Ids if the Users changed.
                if (previousUserId != newUserId)
                {
                    Debug.WriteLine("\n\nThe Usernames changed!!\n\n");
                    // Set the previous User's CastMemberUserId to 0 if that User exists.
                    if (previousUserId != "")
                        db.Users.Find(previousUserId).CastMemberUserID = 0;

                    // Only do this if there was a User selected.  Links the Cast Member and
                    // User together by updated their associated databases.
                    if (newUserId != "")
                    {
                        // Link the Cast Member to the User
                        currentCastMember.CastMemberPersonID = userId;

                        // Get the selected User.
                        var selectedUser = db.Users.Find(userId);

                        // Update the User's Cast Member Id column with castMemberId
                        selectedUser.CastMemberUserID = castMember.CastMemberID;

                        // Save the changes
                        db.Entry(selectedUser).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    // When there is no User selected, remove the reference to the User for this cast member.
                    else
                        currentCastMember.CastMemberPersonID = null;
                }

                if (file != null && file.ContentLength > 0)
                {
                    currentCastMember.PhotoId = PhotoController.CreatePhoto(file, castMember.Name); // Call CreatePhoto method from Photocontroller and assign return value (int photo.PhotoId) to currentCastMember.PhotoId
                }
                else
                {
                    currentCastMember.PhotoId = oldPhotoId; //new attribute PhotoId
                }
                //castMember.CastMemberPersonID = db.Users.Find(userId).Id;
                //db.Entry(castMember).State = EntityState.Modified;
                db.Entry(currentCastMember).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                // If the ModelState is invalid for some reason, make sure to retain the Cast Member's User selection.
                //ViewData["dbUsers"] = new SelectList(db.Users.ToList(), "Id", "UserName", db.CastMembers.Find(castMember.CastMemberID).CastMemberPersonID);

                // The same thing can be acheived if I expand this line's scope: var currentCastMember = db.CastMembers.Find(castMember.CastMemberID);
                // and then use new SelectList(db.Users.ToList(), "Id", "UserName", currentCastMember.CastMemberPersonID);
                // Now that I think of it, I'm using the value of currentCastMember whether or not the ModelState is valid or not,
                // So I think I'm safe to expand it's scope.

                ViewData["dbUsers"] = new SelectList(db.Users.ToList(), "Id", "UserName", currentCastMember.CastMemberPersonID);
            }

            return View(castMember);
        }


        // GET: CastMembers/Delete/5
        [TheatreAuthorize(Roles = "Admin")]

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CastMember castMember = db.CastMembers.Find(id);
            if (castMember == null)
            {
                return HttpNotFound();
            }
            if (castMember.CastMemberPersonID != null)
                ViewBag.CurrentUser = db.Users.Find(castMember.CastMemberPersonID).UserName;
            return View(castMember);
        }

        // POST: CastMembers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [TheatreAuthorize(Roles = "Admin")]

        public ActionResult DeleteConfirmed(int? id)
        {
            CastMember castMember = db.CastMembers.Find(id);

            // Before the cast member is removed.  Set the associated User CastMemberUserId to 0 if a User was assigned.
            if (castMember.CastMemberPersonID != null)
                db.Users.Find(castMember.CastMemberPersonID).CastMemberUserID = 0;

            // Before cast memeber is removed, search for associated parts and set Person_CastMemberId to NULL
            foreach (var i in db.Parts.Where(p => p.Person.CastMemberID == id))
            {
                i.Person = null;
            }

            db.CastMembers.Remove(castMember);

            // PROBABLY NOT NEEDED

            // Remove the ModelState Error when the cast member is deleted.  Now the user associated with this
            // Deleted Cast Member can be assigned without creating an error.
            //string username = db.Users.Where(x => x.CastMemberUserID == id).First().UserName;
            //if (ModelState.ContainsKey(username))
            //    ModelState[username].Errors.Clear();

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

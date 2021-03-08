using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using TheatreCMS.Models;

namespace TheatreCMS.Controllers
{
    [Authorize]
    public class MessagesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Messages
        public ActionResult Index(string searchQuery)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            ApplicationUser currentUser = userManager.FindById(User.Identity.GetUserId());
            var filteredUsers = db.Users.OrderBy(i => i.LastName).ThenBy(i => i.FirstName);
            if (!User.IsInRole("Admin"))
            {
                filteredUsers = (IOrderedQueryable<ApplicationUser>)filteredUsers.Where(i => i.Role == "Admin");
            }
            var UserList = filteredUsers.Select(i => new SelectListItem
            {
                Value = i.Id,
                Text = i.LastName + ", " + i.FirstName
            });
            ViewData["listOfUsers"] = new SelectList(UserList, "Value", "Text");
            ViewData["currentUserId"] = currentUser.Id;
            ViewData["currentUserName"] = currentUser.LastName + ", " + currentUser.FirstName;

            // Get all messages for the user
            var messages = db.Messages.Where(i => (i.SenderId == currentUser.Id && i.SenderPermanentDelete != true) || (i.RecipientId == currentUser.Id && i.RecipientPermanentDelete != true)).OrderByDescending(i => i.SentTime).ToList();

            // If there is a search query, filter messages that match the search
            if (!string.IsNullOrEmpty(searchQuery))
            {
                var users = db.Users.Where(u => u.FirstName.Contains(searchQuery) || u.LastName.Contains(searchQuery)).Select(u => u.Id).ToArray();


                messages = messages.Where(m => m.Subject.ToLower().Contains(searchQuery.ToLower()) || m.Body.ToLower().Contains(searchQuery.ToLower()) || users.Any(u => u == m.SenderId) || users.Any(u => u == m.RecipientId)).ToList();
                ViewData["searchQuery"] = searchQuery;
            }


            

            return View(messages);
        }

        // GET: Messages/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Message message = db.Messages.Find(id);
            if (message == null)
            {
                return HttpNotFound();
            }
            return View(message);
        }

        // GET: Messages/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Messages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MessageId,RecipientId,IsViewed,ParentId,Subject,Body")] Message message)
        {
            if (ModelState.IsValid)
            {
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                ApplicationUser currentUser = userManager.FindById(User.Identity.GetUserId());
                message.SenderId = currentUser.Id;
                message.SentTime = DateTime.Now;

                if(message.Body == null)
                {
                    message.Body = "";
                }
                if (message.Subject == null)
                {
                    message.Subject = "";
                }

                db.Messages.Add(message);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(message);
        }

        // GET: Messages/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Message message = db.Messages.Find(id);
        //    if (message == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(message);
        //}

        // POST: Messages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //In this particular controller, Edit is only called to update IsViewed via AJAX
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int MessageId)
        {
            Message message = db.Messages.Find(MessageId);
            if (message == null)
            {
                return HttpNotFound();
            }
            message.IsViewed = DateTime.Now;
            db.Entry(message).State = EntityState.Modified;
            db.SaveChanges();
            return Json(new { success = true });

        }

        // GET: Messages/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Message message = db.Messages.Find(id);
            if (message == null)
            {
                return HttpNotFound();
            }
            return View(message);
        }

        // POST: Messages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Message message = db.Messages.Find(id);
            //db.Messages.Remove(message);
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            ApplicationUser currentUser = userManager.FindById(User.Identity.GetUserId());

            string currentId = currentUser.Id.ToString();

            if (message.SenderId == currentId)
            {
                message.SenderDeleted = DateTime.Now;
            } 
            if (message.RecipientId == currentUser.Id)
            {
                message.RecipientDeleted = DateTime.Now;
            }
            
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        // GET: Messages/Recover/5
        public ActionResult Recover(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Message message = db.Messages.Find(id);
            if (message == null)
            {
                return HttpNotFound();
            }
            return View(message);
        }

        // POST: Messages/Recover/5
        [HttpPost, ActionName("Recover")]
        [ValidateAntiForgeryToken]
        public ActionResult RecoverConfirmed(int id)
        {
            Message message = db.Messages.Find(id);
            //db.Messages.Remove(message);
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            ApplicationUser currentUser = userManager.FindById(User.Identity.GetUserId());

            string currentId = currentUser.Id.ToString();

            if (message.SenderId == currentId)
            {
                message.SenderDeleted = null;
            }
            if (message.RecipientId == currentUser.Id)
            {
                message.RecipientDeleted = null;
            }

            db.SaveChanges();
            return RedirectToAction("Index");
        }


        // GET: Messages/PermDel/5
        public ActionResult PermDel(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Message message = db.Messages.Find(id);
            if (message == null)
            {
                return HttpNotFound();
            }
            return View(message);
        }

        // POST: Messages/PermDel/5
        [HttpPost, ActionName("PermDel")]
        [ValidateAntiForgeryToken]
        public ActionResult PermDelConfirmed(int id)
        {
            Message message = db.Messages.Find(id);

            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            ApplicationUser currentUser = userManager.FindById(User.Identity.GetUserId());

            string currentId = currentUser.Id.ToString();

            if (message.SenderId == currentId)
            {
                message.SenderPermanentDelete = true;
            }
            if (message.RecipientId == currentUser.Id)
            {
                message.RecipientPermanentDelete = true;
            }

            if (message.SenderPermanentDelete && message.RecipientPermanentDelete)
            {
                db.Messages.Remove(message);
            }
            

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

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TheatreCMS.Enum;
using TheatreCMS.Models;

namespace TheatreCMS.Controllers
{   
    /* Restricting access to only Admins. */
    [Authorize(Roles = "Admin")]
    public class PartController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Part
        public ActionResult Index()
        {
            FilterOptions();

            return View(db.Parts.ToList());
        }

        /* This method gets involked when a user makes a selection from one of the drop down lists. */
        [HttpPost]
        /* The input parameters are set to a default value initially. When a selection is made from the drop down lists, it changes the default value to the
         * 'value' (which is the Id for Productions and CastMembers, and the enum 'Type' string name for Roles) from the rendered HTML. */
        public ActionResult Index(int ProductionsID = 0, int CastMembersID = 0 , string Roles = "")
        {
            FilterOptions();

            /* Depending on which drop down list item(s) are selected, it determines which of the conditions below will be executed. */
            /* These if statements figure out which query to do based on the drop down selected. */
            List<Part> queriedPartsList = db.Parts.ToList();

            if (ProductionsID != 0)
            {
               queriedPartsList = queriedPartsList.Where(part => part.Production.ProductionId == ProductionsID).ToList();
            }
            if (CastMembersID != 0)
            {
                queriedPartsList = queriedPartsList.Where(part => part.Person.CastMemberID == CastMembersID).ToList();
            }
            if (Roles != "")
            {
                queriedPartsList = queriedPartsList.Where(part => part.Type.ToString() == Roles).ToList();
            }

            return View(queriedPartsList);
        }

        /* This method encapsulates filtering options and is called in both the GET and POST Index() methods. */
        public void FilterOptions()
        {
            /* From the Parts Db, group by ProductionId, and select the first ProductionId found and add it to a list. This query prevents duplicate 
            * Production Titles in the Productions drop down list on the Parts Index page. */
            List<Part> partProdQueryList = db.Parts.GroupBy(part => new { part.Production.ProductionId }).Select(i => i.FirstOrDefault()).ToList();

            /*This is what helps populate the Productions drop down list. */
            var filterProds = partProdQueryList.Select(i => new SelectListItem
            {
                /* The Value is what sets the 'value' attribute in the rendered HTML and the Text is the text in between the options 
                 * tag from the drop down list. */
                Value = i.Production.ProductionId.ToString(),
                Text = i.Production.Title
            });
            /* ViewData returns a dictionary based on the Value and Text above. */
            ViewData["ProductionsID"] = new SelectList(filterProds, "Value", "Text");

            List<Part> partCastMemQueryList = db.Parts.GroupBy(part => new { part.Person.CastMemberID }).Select(i => i.FirstOrDefault()).ToList();

            var filterCastMem = partCastMemQueryList.Select(i => new SelectListItem
            {
                Value = i.Person.CastMemberID.ToString(),
                Text = i.Person.Name
            });
            ViewData["CastMembersID"] = new SelectList(filterCastMem, "Value", "Text");

            List<Part> partRolesQueryList = db.Parts.GroupBy(part => new { part.Type }).Select(i => i.FirstOrDefault()).ToList();

            var filterRoles = partRolesQueryList.Select(i => new SelectListItem
            {
                Value = i.Type.ToString(),
                Text = i.Type.ToString()
            });
            ViewData["Roles"] = new SelectList(filterRoles, "Value", "Text");
        }

        /* This method will reset the View by calling the GET Index method. It will remove a users' selections from the drop down lists, resetting the drop downs
         * to display "All Productions" "All Cast Members" "All Roles". */
        public ActionResult ResetFilters()
        {
            return RedirectToAction("Index");
        }

        // GET: Part/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Part part = db.Parts.Find(id);
            if (part == null)
            {
                return HttpNotFound();
            }
            return View(part);

        }

        // GET: Part/Create
        [Authorize(Roles ="Admin")]
        public ActionResult Create()
        {
            ViewData["dbUsers"] = new SelectList(db.Users.ToList(), "Id", "UserName");

            ViewData["CastMembers"] = new SelectList(db.CastMembers.ToList(), "CastMemberId", "Name");

            ViewData["Productions"] = new SelectList(db.Productions.ToList(), "ProductionId", "Title");
            return View();
        }

        // POST: Part/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PartID,Production,Person,Character,Type,Details")] Part part)
        {
            //=== VALIDATION - PRODUCTION check if selected from dropdown
            if (Request.Form["Production"] != "")
            {
                int productionID = Convert.ToInt32(Request.Form["Production"]);
                var production = db.Productions.Find(productionID);
                part.Production = production;
                ModelState.Remove("Production"); // manual remove error - throws by default due to Part model [Required] validation can't match to dropdown
            }
            //=== VALIDATION - PERSON check if selected from dropdown
            if (Request.Form["Person"] != "")
            {
                int castID = Convert.ToInt32(Request.Form["Person"]);
                var person = db.CastMembers.Find(castID);
                part.Person = person;
                ModelState.Remove("Person"); // manual remove error - throws by default due to Part model [Required] validation can't match to dropdown
            }
            //=== VALIDATION - FORM
            if (ModelState.IsValid)
            {
                //=== IS VALID - lookup production and cast member objects based on form value "Id" for each
                db.Parts.Add(part);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                //=== NOT VALID - reload dropdown menus then return part to view 
                ViewData["dbUsers"] = new SelectList(db.Users.ToList(), "Id", "UserName");
                ViewData["CastMembers"] = new SelectList(db.CastMembers.ToList(), "CastMemberId", "Name");
                ViewData["Productions"] = new SelectList(db.Productions.ToList(), "ProductionId", "Title");
                return View(part);
            }            
        }

        [Authorize(Roles = "Admin")]
        // GET: Part/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                 return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Part part = db.Parts.Find(id);
            if (part == null)
            {
                return HttpNotFound();
            }
            Part person = db.Parts.Find(id);
            if (person == null)
            {
                return HttpNotFound();
            }
            Part production = db.Parts.Find(id);
            if (production == null)
            {
                return HttpNotFound();
            }

			ViewData["Productions"] = new SelectList(db.Productions, "ProductionId", "Title", part.Production.ProductionId);
			
			ViewData["CastMembers"] = new SelectList(db.CastMembers, "CastMemberId", "Name", part.Person.CastMemberID);

            return View(part);
        }

        // POST: Part/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PartID,Production,Person,Character,Type,Details")] Part part)
        {

            int castID = Convert.ToInt32(Request.Form["CastMembers"]);
            int productionID = Convert.ToInt32(Request.Form["Productions"]);
            if (ModelState.IsValid)
            {
                var currentPart = db.Parts.Find(part.PartID);
                    currentPart.Character = part.Character;
                    currentPart.Type = part.Type;
                    currentPart.Details = part.Details;

                ViewData["Productions"] = new SelectList(db.Productions.ToList(), "ProductionId", "Title");

                ViewData["CastMembers"] = new SelectList(db.CastMembers.ToList(), "CastMemberID", "Name");

				var production = db.Productions.Find(productionID);
                
                var person = db.CastMembers.Find(castID);
                
                currentPart.Production = production;
                db.Entry(currentPart.Production).State = EntityState.Modified;
                db.SaveChanges();
                currentPart.Person = person;
                db.Entry(currentPart.Person).State = EntityState.Modified;
                db.SaveChanges();
                db.Entry(currentPart).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(part);
        }

        // GET: Part/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Part part = db.Parts.Find(id);
            if (part == null)
            {
                return HttpNotFound();
            }
            return View(part);
        }

        // POST: Part/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Part part = db.Parts.Find(id);
            db.Parts.Remove(part);
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

        // POST: Deletes multiple parts at once
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult PartsToDelete (IEnumerable<int> PartIdsToDelete)
        {
            List<Part> partsToDelete = db.Parts.Where(x => PartIdsToDelete.Contains(x.PartID)).ToList();
            foreach (var part in partsToDelete)
            {
                db.Parts.Remove(part);
            }
                db.SaveChanges();
            return Redirect("Index");
        }
    }
}

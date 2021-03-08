using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TheatreCMS_B.Controllers
{
    public class ProductionsController : Controller
    {
        //This is the index page
        // GET: Productions
        public ActionResult Events()
        {
            return View();
        }

        // GET: Productions/Subscribe
        public ActionResult Subscribe()
        {
            return View();
        }

        // GET: Productions/Calendar
        public ActionResult Calendar()
        {
            return View();
        }

        // GET: Productions/TicketInfo
        public ActionResult TicketInfo()
        {
            return View();
        }

        // GET: Productions/PastSeasons
        public ActionResult PastSeasons()
        {
            return View();
        }
    }
}
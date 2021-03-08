using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TheatreCMS.Helpers;
using TheatreCMS.Models;

namespace TheatreCMS.Controllers
{
    public class DeveloperController : Controller
    {
        // GET: Developer
        [Authorize(Roles = "Admin")]
        public ActionResult Templates()
        {
            return View();
        }
        /// <summary>
        /// Stores the state of the Bug Report widget in AdminSettings.json
        /// </summary>
        /// <param name="tab">BugReportTab object</param>
        /// <returns></returns>
        [HttpPost]

        public ActionResult BugTabStateUpdate(BugReportTab bugreport)
        {
            string filepath = Server.MapPath(Url.Content("~/AdminSettings.json"));

            //Get the current settings
            AdminSettings currentAdminSettings = AdminSettingsReader.CurrentSettings();

            //Insert our new value
            currentAdminSettings.BugReport = bugreport;

            //Convert to json
            string newJson = JsonConvert.SerializeObject(currentAdminSettings, Formatting.Indented);

            //Write to file
            using (StreamWriter writer = new StreamWriter(filepath))
            {
                writer.Write(newJson);
            }
            //make Ajax happy with a success message
            return Json(new { success = true, message = $"tab_open: {bugreport.tab_open}" });
        }
    }
}
    






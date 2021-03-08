using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TheatreCMS.Models;
using Newtonsoft.Json;
using System.IO;

namespace TheatreCMS.Helpers
{
    public class AdminSettingsReader
    {
        public static AdminSettings CurrentSettings()
        {
            AdminSettings currentSettings = new AdminSettings();     
            string filepath = System.Web.HttpContext.Current.Server.MapPath("~/AdminSettings.json");
            string result = string.Empty;
            using (StreamReader r = new StreamReader(filepath))
            {
                result = r.ReadToEnd();
            }
            currentSettings = JsonConvert.DeserializeObject<AdminSettings>(result);
            return currentSettings;
        }
    }
}
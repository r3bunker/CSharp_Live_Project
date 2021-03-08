using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace TheatreCMS.Models
{
    //Many the classes below are designed to be read by the helper method AdminSettingsReader().
    //Datatypes besides int/string or classes/lists of int/strings are liable to throw errors.
    //Any Classes/properties not meant to be used in Json string, is recommened to have [JsonIgnore] above,
    //or to place it in AdminSettings
    public class AdminSettings
    {
        [JsonProperty("season_productions")]
        public SeasonProductions season_productions { get; set; }

        public Footer FooterInfo { get; set; }

        [JsonProperty("models_missing_photos")]
        public ModelsWithoutPhotos models_missing_photos { get; set; }

        [JsonProperty("recent_definition")]
        public RecentDefinition recent_definition { get; set; }

        [JsonProperty("on_stage")]
        public int on_stage { get; set; }

        [Required(ErrorMessage = "Please enter current season number")]     // for current season validation
        [JsonProperty("current_season")]
        public int current_season { get; set; }

        [JsonProperty("current_productions")]
        public List<int> current_productions { get; set; }     //a list of production IDs for current season

        public BugReportTab BugReport { get; set; }

        [JsonIgnore]
        public static string filepath = HttpContext.Current.Server.MapPath("~/AdminSettings.json");

        /// <summary>This method collects information from the Json file.</summary>
        /// <returns>A deserialized AdminSettings object variable</returns>
        public static AdminSettings GetCurrentSettings()
        {
            string result = string.Empty;
            using (StreamReader r = new StreamReader(filepath))
            {
                result = r.ReadToEnd();
            }
            AdminSettings currentSettings = JsonConvert.DeserializeObject<AdminSettings>(result);
            return currentSettings;
        }

        /// <summary> This method takes in a string (a serialized reperesentation of an AdminSettings Object) and writes it to Json file.</summary>
        /// <param name="SerializedAdminObject">A serialized AdminSettings object</param>
        /// <return>There is no return for this method</return>
        public static void SetAdminSettings(string SerializedAdminObject)
        {
            using (StreamWriter r = new StreamWriter(filepath))
            {
                r.Write(SerializedAdminObject);
            }
        }

        /// <summary>This method takes in an AdminObject, converts it to a serialzied string, and writes it to Json file.</summary>
        /// <param name="AdminObject">A (non-serialized) AdminSettings object</param>
        /// <return>There is no return for this method</return>
        public static void SetAdminSettings(AdminSettings AdminObject)
        {
            string SerializedAdminObject = JsonConvert.SerializeObject(AdminObject);
            SetAdminSettings(SerializedAdminObject);
        }
    }

    public class SeasonProductions
    {
        public int fall { get; set; }
        public int winter { get; set; }
        public int spring { get; set; }
    }
    public class RecentDefinition      //Lets Admin Define what they consider to be "recent", such as recent subscribers or productions
    {
        //// 0 = Date
        //// 1 = Span
        //public byte selection { get; set; }     // Span or Date
        public bool bUsingSpan { get; set; }    // If True, using Span instead of Date

        public int? span { get; set; }        //Number of months in the past

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime date { get; set; }  // Earliest date for what is considered recent
    }

    public class Footer                  //All Information presented in Dashboard footer
    {
        public string AddressStreet { get; set; }
        public string AddressCityStateZip { get; set; }
        public string PhoneSales { get; set; }
        public string PhoneGeneral { get; set; }
        public int CopyrightYear { get; set; }
        public string EmailAddress { get; set; }
    }

    public class ModelsWithoutPhotos        //Used for methods regarding missing photos in models
    {
        public List<int> Productions { get; set; }
        public List<int> CastMembers { get; set; }
        public List<int> Sponsors { get; set; }
        public List<int> ProductionPhotos { get; set; }
    }

    public class BugReportTab
    {
        public bool tab_open { get; set; }
    }
}


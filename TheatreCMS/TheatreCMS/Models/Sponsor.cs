using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace TheatreCMS.Models
{
    public class Sponsor
    {
        [Key]
        public int SponsorId { get; set; }  // sponsor primary key
        public string Name { get; set; }    // sponsor name

       // public int? Height { get; set; }    // logo display height (may be different from original)
        //public int? Width { get; set; }     // logo display width (may be different from original)
        public bool Current { get; set; }   // active sponsor
        public string Link { get; set; }        //url of sponsor website

        //Declared Photo in model to pull in attributes height and width directly.  Also changed from LogoId to PhotoId since we get that directly. 
        [Display(Name = "Logo Image")]
        public int? PhotoId { get; set; }     // ID of photo.cs object
        public virtual Photo Photo { get; set; }
    }
} 
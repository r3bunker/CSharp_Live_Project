using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TheatreCMS.Models
{
    public class DisplayInfo
    {
        [Key]
        public int InfoId { get; set; }         // info primary key
        public string Title { get; set; }       // info for which page content relates to
        public string TextContent { get; set; } // content for the aforementioned page


        [Display(Name = "Image")]
        public int? PhotoId { get; set; }     // ID of photo.cs objec
        public virtual Photo Photo { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TheatreCMS.Models
{
    public class DisplayLinks
    {
        [Key]
        public int LinkId { get; set; }     // link primary key
        public string Name { get; set; }    // link title
        public string Text { get; set; }    // link display text
        public string Link { get; set; }    // link url
    }
}
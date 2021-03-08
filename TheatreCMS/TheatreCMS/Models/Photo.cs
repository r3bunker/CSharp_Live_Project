using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TheatreCMS.Models
{
    public class Photo
    {
        public int PhotoId { get; set; }
        public byte[] PhotoFile { get; set; }
        public int OriginalHeight { get; set; }
        public int OriginalWidth { get; set; }
        public string Title { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using TheatreCMS.Enum;

namespace TheatreCMS.Models
{
    public class ContentSection
    {
        [Key]
        public int SectionId { get; set; }              // content section primary key
        public ContentEnum ContentType { get; set; }    // content type
        public int ContentId { get; set; }              // Id of associated content
        public string CssId { get; set; }               // CSS class name for desired styling

    }

}
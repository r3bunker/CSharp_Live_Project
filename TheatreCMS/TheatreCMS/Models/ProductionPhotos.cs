using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TheatreCMS.Models
{
    public class ProductionPhotos
    {
        [Key]
        public int ProPhotoId { get; set; }         // production photo primary key
        [Display(Name ="Photo")]
        public int? PhotoId { get; set; }            // primary key for associated photo
        public string Title { get; set; }           // photo title
        public string Description { get; set; }     // photo description

        [InverseProperty ("ProductionPhotos")]
        public virtual Production Production { get; set; }  //associated production

    }
}
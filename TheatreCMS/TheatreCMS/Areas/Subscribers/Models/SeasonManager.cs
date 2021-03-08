using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TheatreCMS.Models;

namespace TheatreCMS.Areas.Subscribers.Models
{
    public class SeasonManager
    {
        [Key]
        public int SeasonManagerId { get; set; }    // season manager primary key
        
        [Display(Name = "Number of Seats Available")]
        public int NumberSeats { get; set; }        // number of seats available to book for each production
        
        [Display(Name = "Season Booking Complete")]
        public bool BookedCurrent { get; set; }
        
        [Display(Name = "Fall Production Name")]// 
        public string FallProd { get; set; }        // production name for fall (--> string to virtual)

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:MM-dd-yyyy hh:mm tt}", ApplyFormatInEditMode = true)]
        [Display(Name = "Fall Production Time")]
        public DateTime? FallTime { get; set; }     // chosen date and time for fall production 

        [Display(Name = "Booked for Fall")]
        public bool BookedFall { get; set; }        // fall booking approved

        [Display(Name = "Winter Production Name")]
        public string WinterProd { get; set; }      // production name for winter (--> string to virtual)

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:MM-dd-yyyy hh:mm tt}", ApplyFormatInEditMode = true)]
        [Display(Name = "Winter Production Time")]
        public DateTime? WinterTime { get; set; }   // chosen date and time for winter production 

        [Display(Name = "Booked for Winter")]
        public bool BookedWinter { get; set; }      // winter booking approved

        [Display(Name = "Spring Production Name")]
        public string SpringProd { get; set; }      // production name for spring (--> string to virtual)

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:MM-dd-yyyy hh:mm tt}", ApplyFormatInEditMode = true)]
        [Display(Name = "Spring Production Time")]
        public DateTime? SpringTime { get; set; }   // chosen date and time for spring production 

        [Display(Name = "Booked for Spring")]
        public bool BookedSpring { get; set; }      // spring booking approved

        [Required]
        [Display(Name = "Season Manager Person")]
        public virtual ApplicationUser SeasonManagerPerson { get; set; }    // associated user

        [Required]
        [Display(Name = "Season Being Managed")]
        public int Season { get; set; }             // Indicates which season the manager is for

    }
}
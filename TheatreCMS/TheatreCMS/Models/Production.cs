using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace TheatreCMS.Models
{
    public class Production
    {
        [Key]
        public int ProductionId { get; set; }       // production primary key
        [Required]
        public string Title { get; set; }           // production title
        public string Playwright { get; set; }      // production playwright
        public string Description { get; set; }     // production description

        [Display(Name = "Show Runtime (min)")]
        public int Runtime { get; set; }            // show runtime

        [Display(Name = "Opening Day")]
        public DateTime OpeningDay { get; set; }    // production opening day

        [Display(Name = "Closing Day")]
        public DateTime ClosingDay { get; set; }    // production closing day

        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        [Display(Name = "Evening Showtime")]
        public DateTime? ShowtimeEve { get; set; }  // production evening showtime

        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        [Display(Name = "Matinee Showtime")]
        public DateTime? ShowtimeMat { get; set; }  // production matinee showtime

        [Display (Name = "Ticket Link")]
        public string TicketLink { get; set; }      // url for purchasing tickets
        public int Season { get; set; }             // production season number

        [Display(Name = "World Premiere")]
        public bool IsWorldPremiere { get; set; }   // first time produced in the world

        [Display(Name = "Promo Photo")]
        public virtual ProductionPhotos DefaultPhoto { get; set; }                  // promotional photo for this production

        public virtual ICollection<Part> Parts { get; set; }                        // all cast member parts for this production

        public virtual ICollection<CalendarEvent> Events { get; set; }              // associated production events

        [InverseProperty ("Production")]
        public virtual ICollection<ProductionPhotos> ProductionPhotos { get; set; } // associated production photos

        [Display(Name = "Currently Showing")]
        public bool IsCurrentlyShowing()
        {
            return DateTime.Now >= OpeningDay && DateTime.Now <= ClosingDay;
        }
        //public bool IsCurrentlyShowing;
        //public bool IsCurrentlyShowing()            //Method for checking which shows are currently showing based off of current date and if it falls between or equal to the opening and closing day
        //{
        //    if (DateTime.Now >= OpeningDay && DateTime.Now <= ClosingDay)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        //public static implicit operator object(Production v)
        // {
        //  throw new NotImplementedException();
        //}
    }

}
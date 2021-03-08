using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using TheatreCMS.Models;

namespace TheatreCMS.Areas.Subscribers.Models
{
    public class Subscriber
    {
        [Key]
        [ForeignKey("SubscriberPerson")]
        public string SubscriberId { get; set; }        // subscriber primary key
        
        [Required]
        [Display(Name = "Current Subscriber")]
        public bool CurrentSubscriber { get; set; }     // subscriber has purchased current season
        
        [Required]
        [Display(Name = "Has Renewed")]
        public bool HasRenewed { get; set; }            // subscriber has purchased next season (or simply creating new season manager would suffice)
        public bool Newsletter { get; set; }            // subscriber signed up for newsletter
        [Required]  
        [Display(Name = "Recent Donor")]
        public bool RecentDonor { get; set; }           // subscriber donated "recently" as defined by the settings file
       
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Last Donated")]
        public DateTime? LastDonated { get; set; }      // date of last donation
        [Display(Name = "Last Donation Amt")]
        public decimal? LastDonationAmt { get; set; }   // amount of last donation
        [Display(Name = "Special Requests")]
        public string SpecialRequests { get; set; }     // general special needs for bookings
        public string Notes { get; set; }               // general notes
        [Required]
        public virtual ApplicationUser SubscriberPerson { get; set; }   // associated user
        public virtual SubscriptionPlan SubscriptionPlan { get; set; }
    }
}
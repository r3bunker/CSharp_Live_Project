using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace TheatreCMS.Areas.Subscribers.Models
{
    public class SubscriptionPlan
    {
        [Key]
        public int PlanId { get; set; }
        public string SubscriptionLevel { get; set; }
        public decimal PricePerYear { get; set; }
        public int NumberOfShows { get; set; }
        public virtual List<Subscriber> Subscribers { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TheatreCMS.Models
{
        public class RentalRequest
        {
                public RentalRequest()
                {
                        RequestedTime = DateTime.Now;
                }

                [Key]
                public int RentalRequestId { get; set; }

                [Display(Name = "Contact Person")]
                public string ContactPerson { get; set; }
                public string Company { get; set; }
                [Display(Name = "Requested Time")]
                public DateTime RequestedTime { get; set; }
                [Display(Name = "Start Time")]
                public DateTime StartTime { get; set; }
                [Display(Name = "End Time")]
                public DateTime EndTime { get; set; }
                [Display(Name = "Project Info")]
                public string ProjectInfo { get; set; }
                public string Requests { get; set; }
                [Display(Name = "Rental Code")]
                public int RentalCode { get; set; }
                public bool Accepted { get; set; }
                [Display(Name = "Contract Signed")]
                public bool ContractSigned { get; set; }

                public TimeSpan GetRentalDuration()
                {
                        TimeSpan duration = EndTime - StartTime;
                        return duration;
                }

                public TimeSpan GetTimeRemaining()
                {
                        TimeSpan duration = DateTime.Now - EndTime;
                        return duration;
                }
        }
}

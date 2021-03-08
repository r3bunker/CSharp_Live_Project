using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TheatreCMS.Models
{
    public class Award
    {
        [Key]
        public int AwardId { get; set; }

        [Required]
        public int Year { get; set; }
      //  public SelectList YearList { get; }  // Do not use this anymore

        [Required]
        [Display(Name = "Award Name")]
        public string Name { get; set; }
       
        [Required]
        public AwardType? Type { get; set; }
       

        [Required]
        public string Category { get; set; }
        
        public string Recipient { get; set; }

        
        public Nullable<int> ProductionId { get; set; }
        public virtual Production Production { get; set; }

        
        public Nullable<int> CastMemberId { get; set; }
        public virtual CastMember CastMember { get; set; }


        [Display(Name = "Other Information")]
        public string OtherInfo { get; set; }
    }


    public enum AwardType
    {
        Award,
        Finalist,
        Other
    }
}   
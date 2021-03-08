using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using WebGrease;

namespace TheatreCMS_B.Models
{
    public class Production
    {
        [Key] //Setting it as a key
        public int ProductionId { get; set; } //Primary key for product
        [Required] //A value in the Title is required to create a Prodcution
        public string Title { get; set; } 
        public string Description { get; set; }
        [Display(Name = "Opening day")] 
        public DateTime StartDate { get; set; }
        [Display(Name = "Closing day")]
        public DateTime EndDate { get; set; }
        [Display(Name = "Show runtime (min)")] //Making it easier for people to understand
        public int RunTime { get; set; }
        public Status Status { get; set; }

        ICollection<ProductionEvent> ProductionEvent { get; set; }
    }
    public enum Status
    {
        past,
        current,
        upcoming,
    };
}
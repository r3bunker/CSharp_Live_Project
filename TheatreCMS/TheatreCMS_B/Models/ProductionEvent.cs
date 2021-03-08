using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TheatreCMS_B.Models
{
    public class ProductionEvent
    {
        [Key]
        public int EventId { get; set; }
        public string Title { get; set; }
        [Column(TypeName ="datetime")]
        public DateTime StartDate { get; set; }
        [Column(TypeName ="datetime")]
        public DateTime EndDate { get; set; }
        public string Notes { get; set; }
        public int? TicketsAvailable { get; set; }
        public Production Production { get; set; }
    }
}
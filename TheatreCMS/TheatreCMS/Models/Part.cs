using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using TheatreCMS.Enum;


namespace TheatreCMS.Models
{
    public class Part
    {
        [Key]
        public int PartID { get; set; }                     // part primary key

        //Play attribute needs help in the Create() action of the Part controller
        [Required(ErrorMessage = "Please select a valid production.")]
        public virtual Production Production { get; set; }  // associated production

        public string Character { get; set; }               // character name played by Person on Production

        //Type attribute needs help in the Create() action of the Part controller
        public PositionEnum Type { get; set; }              // job position of Person on Production

        [Required(ErrorMessage = "Please select a valid Cast Member.")]
        public virtual CastMember Person { get; set; }      // associated cast member
        public string Details { get; set; }                 // additional details for part

    }
}
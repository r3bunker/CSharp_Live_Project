using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using TheatreCMS.Enum;
using System.Drawing;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheatreCMS.Models
{
    public class CastMember
    {
        [Key]
        public int CastMemberID { get; set; }           // cast member primary key
        public string Name { get; set; }                // cast member display name
        public int? YearJoined { get; set; }            // year that permenant cast member joined theater
        public PositionEnum MainRole { get; set; }      // position at theater
        public string Bio { get; set; }                 // cast member biographical excerpt

        //Photo attribute needs work in the Create() action of the CastMemembersController
        /* public byte[] Photo { get; set; } */              // cast member photo

        public int? PhotoId { get; set; }
        


        public bool CurrentMember { get; set; }         // active permanent or temporary cast member

        //Parts attribute needs work in the Create() action of the CastMemembersController
        public virtual List<Part> Parts { get; set; }   // list of parts played by cast member

        /* Need to find a way to explicitly match a CastMember's User account to their ApplicationUser object, 
        If a castmember signs up for an account, ensure that for ApplicationUser user "=" CastMember castMember,
        user.CastMemberPersonID = castMembe.CastMemberPersonID */
        //public virtual ApplicationUser CastMemberPerson { get; set; } 
        public string CastMemberPersonID { get; set; }  // user ID for cast member
        public bool AssociateArtist { get; set;}        // defined by customer
        public bool EnsembleMember { get; set; }        // defined by customer
        public int? CastYearLeft { get; set; }          // year that permenant cast member leaves theater
        public int? DebutYear { get; set; }             // first year that temporary cast member joins theater
    }
}
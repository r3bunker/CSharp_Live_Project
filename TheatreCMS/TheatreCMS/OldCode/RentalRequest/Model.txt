using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;


namespace TheatreCMS.Models
{
    public class RentalRequest
    {
        [Key]
        public int RentalRequestId { get; set; }    // rental request primary key

        [StringLength(40, ErrorMessage = "Error. This field is limited to 40 characters.")]
        [Display(Name = "Contact Person")]
        public string ContactPerson { get; set; }   // name for point of contact

        [Required(ErrorMessage = "Please provide a valid phone number")]
        [Display(Name = "Contact Phone Number")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        public string ContactPhoneNumber { get; set; }

        [Display(Name = "Contact Email address")]
        [Required(ErrorMessage = "Please provide a valid email address")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string ContactEmail { get; set; }

        [StringLength(100, ErrorMessage = "Error. This field is limited to 100 characters.")]
        public string Company { get; set; }         // company requesting the rental

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:MM-dd-yyyy hh:mm tt}", ApplyFormatInEditMode = true)]
        [Display(Name = "Start Time")]
        public DateTime StartTime { get; set; }     // rental start date and time

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:MM-dd-yyyy hh:mm tt}", ApplyFormatInEditMode = true)]
        [Display(Name = "End Time")]
        public DateTime EndTime { get; set; }       // rental end date and time

        [Display(Name = "Project Info")]
        [StringLength(2000, ErrorMessage = "Error. This field is limited to 2000 characters.")]
        public string ProjectInfo { get; set; }     // info on the project using the space

        [StringLength(1000, ErrorMessage = "Error. This field is limited to 1000 characters.")]
        public string Requests { get; set; }        // special requests for rental

        [Display(Name = "Rental Code")]
        public int RentalCode { get; set; }         // rental confirmation number

        [Display(Name = "Accepted")]
        public bool Accepted { get; set; }          // rental request approved

        [Display(Name = "Contract Signed")]
        public bool ContractSigned { get; set; }    // rental contract signed by both parties




       }

    }






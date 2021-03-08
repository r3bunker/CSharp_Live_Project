using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TheatreCMS.ViewModels
{
    public class NewsletterListVm
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Email { get; set; }
         
    }
}
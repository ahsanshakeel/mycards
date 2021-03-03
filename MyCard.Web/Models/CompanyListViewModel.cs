using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyCard.Web.Models
{
    public class CompanyListViewModel
    {
        public string EmailID { get; set; }
        public string CompanyName { get; set; }
        public string DomainName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public int CompanyID { get; set; }
        public int TotalLicenses { get; set; }
        public string RegistrationDate { get; set; }
        public string Country { get; set; }
        public string Indutry { get; set; }
        public string CustomerType { get; set; }
        public string Approved { get; set; }
        public int AccountType { get; set; }
    }
}
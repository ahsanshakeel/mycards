using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyCard.Web.Models
{
    public class CompanyViewModel
    {
        [Required(ErrorMessageResourceType = (typeof(MyCard.Web.Resources.ErrorMessages)), ErrorMessageResourceName = "CompanyNameRequired")]
        [RegularExpression("^([a-zA-Z0-9 .&'-]+)$", ErrorMessageResourceType = typeof(Resources.ErrorMessages), ErrorMessageResourceName = "InvalidCompanyName")]
        [Display(Name = "CompanyName", ResourceType = typeof(MyCard.Web.Resources.CaptionsAll))]
        public string Name { get; set; }
        [Required(ErrorMessageResourceType = (typeof(MyCard.Web.Resources.ErrorMessages)), ErrorMessageResourceName = "EmailRequired")]
        [EmailAddress(ErrorMessageResourceType = (typeof(MyCard.Web.Resources.ErrorMessages)), ErrorMessageResourceName = "EmailValid")]
        [Display(Name = "EmailID", ResourceType = typeof(MyCard.Web.Resources.CaptionsAll))]
        public string EmailID { get; set; }
        [Required(ErrorMessageResourceType = (typeof(MyCard.Web.Resources.ErrorMessages)), ErrorMessageResourceName = "NameRequired")]
        [RegularExpression("^([a-zA-Z0-9 .&'-]+)$", ErrorMessage = "InvalidName")]
        [Display(Name = "CustomerName", ResourceType = typeof(MyCard.Web.Resources.CaptionsAll))]
        public string CustomerName { get; set; }
        [Required(ErrorMessageResourceType = (typeof(MyCard.Web.Resources.ErrorMessages)), ErrorMessageResourceName = "DomainNameRequired")]
        [Display(Name = "DomainName", ResourceType = typeof(MyCard.Web.Resources.CaptionsAll))]
        [RegularExpression(@"((www\.|(http|https|ftp|news|file|)+\:\/\/)?[&#95;.a-z0-9-]+\.[a-z0-9\/&#95;:@=.+?,##%&~-]*[^.|\'|\# |!|\(|?|,| |>|<|;|\)])", ErrorMessageResourceType = typeof(Resources.ErrorMessages), ErrorMessageResourceName = "DomainValid")]
        public string DomainName { get; set; }
        [Required(ErrorMessageResourceType = (typeof(MyCard.Web.Resources.ErrorMessages)), ErrorMessageResourceName = "PhoneRequired")]
        [RegularExpression("^([0-9 -'-]+)$", ErrorMessage = "InvalidPhone")]
        [Display(Name = "Phone", ResourceType = typeof(MyCard.Web.Resources.CaptionsAll))]
        public string Phone { get; set; }
        [Required(ErrorMessageResourceType = (typeof(MyCard.Web.Resources.ErrorMessages)), ErrorMessageResourceName = "AddressRequired")]
        [RegularExpression("^([a-zA-Z0-9 .&'-]+)$", ErrorMessage = "InvalidAddress")]
        [Display(Name = "Address", ResourceType = typeof(MyCard.Web.Resources.CaptionsAll))]
        public string Address { get; set; }
        [Required(ErrorMessageResourceType = (typeof(MyCard.Web.Resources.ErrorMessages)), ErrorMessageResourceName = "CityRequired")]
        [RegularExpression("^([a-zA-Z0-9 .&'-]+)$", ErrorMessage = "InvalidCity")]
        [Display(Name = "City", ResourceType = typeof(MyCard.Web.Resources.CaptionsAll))]
        public string City { get; set; }
        [Required(ErrorMessageResourceType = (typeof(MyCard.Web.Resources.ErrorMessages)), ErrorMessageResourceName = "StateRequired")]
        [RegularExpression("^([a-zA-Z0-9 .&'-]+)$", ErrorMessage = "InvalidState")]
        [Display(Name = "State", ResourceType = typeof(MyCard.Web.Resources.CaptionsAll))]
        public string State { get; set; }
        [Required(ErrorMessageResourceType = (typeof(MyCard.Web.Resources.ErrorMessages)), ErrorMessageResourceName = "ZipRequired")]
        [RegularExpression("^([a-zA-Z0-9 .&'-]+)$", ErrorMessage = "InvalidZip")]
        [Display(Name = "Zip", ResourceType = typeof(MyCard.Web.Resources.CaptionsAll))]
        public string Zip { get; set; }
        public int CompanyID { get; set; }
    }
}
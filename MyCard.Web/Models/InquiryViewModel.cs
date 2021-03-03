using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace MyCard.Web.Models
{
    
    public class InquiryViewModel
    {
        [RegularExpression("^([a-zA-Z0-9 .&'-]+)$", ErrorMessageResourceType = typeof(Resources.ErrorMessages), ErrorMessageResourceName = "InvalidCompanyName")]
        [Display(Name = "CompanyName", ResourceType = typeof(MyCard.Web.Resources.CaptionsAll))]
        public string BusinessName { get; set; }
        [EmailAddress(ErrorMessageResourceType = (typeof(MyCard.Web.Resources.ErrorMessages)), ErrorMessageResourceName = "EmailValid")]
        public string Email { get; set; }
        [RegularExpression("^([a-zA-Z0-9 .&'-]+)$", ErrorMessage = "InvalidName")]
        [Display(Name = "CustomerName", ResourceType = typeof(MyCard.Web.Resources.CaptionsAll))]
        public string CustomerName { get; set; }                
    }

    
}
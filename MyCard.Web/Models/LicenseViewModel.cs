using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyCard.Web.Models
{
    public class LicenseViewModel
    {
        [Required(ErrorMessageResourceType = (typeof(MyCard.Web.Resources.ErrorMessages)), ErrorMessageResourceName = "CompanyNameRequired")]
        [Display(Name = "CompanyName", ResourceType = typeof(MyCard.Web.Resources.CaptionsAll))]
        public string CompanyName { get; set; }
        [Required(ErrorMessageResourceType = (typeof(MyCard.Web.Resources.ErrorMessages)), ErrorMessageResourceName = "NoOfLicenseRequired")]
        [Display(Name = "NoOfLicense", ResourceType = typeof(MyCard.Web.Resources.CaptionsAll))]
        public int NoOfLicense { get; set; }
        [Display(Name = "LicenseStatus", ResourceType = typeof(MyCard.Web.Resources.CaptionsAll))]
        public string LicenseStatus { get; set; }

    }
}
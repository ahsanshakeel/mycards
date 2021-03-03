using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyCard.Web.Models
{
    public class TrialLicenseViewModel
    {
        [Required(ErrorMessageResourceType = (typeof(MyCard.Web.Resources.ErrorMessages)), ErrorMessageResourceName = "NoOfLicenseRequired")]
        [Display(Name = "NoOfLicense", ResourceType = typeof(MyCard.Web.Resources.CaptionsAll))]
        [Range(1, 20, ErrorMessageResourceType = (typeof(MyCard.Web.Resources.ErrorMessages)), ErrorMessageResourceName = "TrialLicenseRange")]
        public int NoOfLicense { get; set; }
    }
}
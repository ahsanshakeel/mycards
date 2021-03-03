using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyCard.Web.Models
{
    public class LicenseTypeViewModel
    {
        [Required(ErrorMessageResourceType = (typeof(MyCard.Web.Resources.ErrorMessages)), ErrorMessageResourceName = "LicenseTypeRequired")]
        [Display(Name = "License Type", ResourceType = typeof(MyCard.Web.Resources.CaptionsAll))]
        public string LicenseType { get; set; }
        
    }
}
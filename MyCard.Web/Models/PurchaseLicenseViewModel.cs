using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyCard.Web.Models
{
    public class PurchaseLicenseViewModel
    {
        [Required(ErrorMessageResourceType = (typeof(MyCard.Web.Resources.ErrorMessages)), ErrorMessageResourceName = "NoOfLicenseRequired")]
        [Display(Name = "NoOfLicense", ResourceType = typeof(MyCard.Web.Resources.CaptionsAll))]
        [Range(1, int.MaxValue, ErrorMessageResourceType = (typeof(MyCard.Web.Resources.ErrorMessages)), ErrorMessageResourceName = "NoOfLicenseRequired")]
        public int NoOfLicense { get; set; }
        public int LicenseTypeId { get; set; }
        public IList<LicenseTypeModel> LicenseTypes { get; set; }
    }
    public class SendEmail
    {
        public string name { get; set; }
        public string Email { get; set; }
        public string companyname { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyCard.Web.Models
{
    public class CompanyLicenseViewModel
    {
        public string CompanyLicenseType { get; set; }
        public IList<LicenseDescriptionModel> LicenseDescriptions { get; set; }
    }
}
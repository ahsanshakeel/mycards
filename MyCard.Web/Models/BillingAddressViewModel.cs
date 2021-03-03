using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyCard.Web.Models
{
    public class BillingAddressViewModel
    {
        [Display(Name = "BillingAddressCaption", ResourceType = typeof(MyCard.Web.Resources.CaptionsAll))]
        [DataType(DataType.MultilineText)]
        public string Address { get; set; }
    }
}
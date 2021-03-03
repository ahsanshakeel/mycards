using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyCard.Web.Models
{
    public class TermsViewModel
    {
        [Display(Name = "TermsAndConidtions", ResourceType = typeof(MyCard.Web.Resources.CaptionsAll))]
        public string TermsAndConidtions { get; set; }
        [Display(Name = "Accepted", ResourceType = typeof(MyCard.Web.Resources.CaptionsAll))]
        public bool Accepted { get; set; }
    }
}
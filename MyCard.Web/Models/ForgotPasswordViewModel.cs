using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyCard.Web.Models
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessageResourceType = (typeof(MyCard.Web.Resources.ErrorMessages)), ErrorMessageResourceName = "EmailRequired")]
        [EmailAddress(ErrorMessageResourceType = (typeof(MyCard.Web.Resources.ErrorMessages)), ErrorMessageResourceName = "EmailValid")]
        [Display(Name = "EmailID", ResourceType = typeof(MyCard.Web.Resources.CaptionsAll))]
        public string Email { get; set; }
    }
}
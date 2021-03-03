using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyCard.Web.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessageResourceType = (typeof(MyCard.Web.Resources.ErrorMessages)), ErrorMessageResourceName = "UserNameRequired")]
        [Display(Name = "UserName", ResourceType = typeof(MyCard.Web.Resources.CaptionsAll))]
        public string UserName { get; set; }
        [Required(ErrorMessageResourceType = (typeof(MyCard.Web.Resources.ErrorMessages)), ErrorMessageResourceName = "PasswordRequired")]
        [Display(Name = "Password", ResourceType = typeof(MyCard.Web.Resources.CaptionsAll))]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name = "RememberMe", ResourceType = typeof(MyCard.Web.Resources.CaptionsAll))]
        public bool RememberMe { get; set; }
    }
}
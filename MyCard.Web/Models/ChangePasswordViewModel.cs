using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyCard.Web.Models
{
    public class ChangePasswordViewModel
    {
        //[Required(ErrorMessageResourceType = (typeof(MyCard.Web.Resources.ErrorMessages)), ErrorMessageResourceName = "OldPasswordRequired")]
        [Display(Name = "OldPassword", ResourceType = typeof(MyCard.Web.Resources.CaptionsAll))]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }
       // [Required(ErrorMessageResourceType = (typeof(MyCard.Web.Resources.ErrorMessages)), ErrorMessageResourceName = "PasswordRequired")]
        [Display(Name = "Password", ResourceType = typeof(MyCard.Web.Resources.CaptionsAll))]
        //[StringLength(255, ErrorMessageResourceType = (typeof(MyCard.Web.Resources.ErrorMessages)), ErrorMessageResourceName = "PasswordLengthErrorMessage", MinimumLength = 8)]
        [DataType(DataType.Password)]
        //[RegularExpression("^(?=.*[A-Za-z])(?=.*\\d)[A-Za-z\\d]{8,}$", ErrorMessageResourceType = typeof(Resources.ErrorMessages), ErrorMessageResourceName = "InvalidPassword")]
        public string Password { get; set; }
        //[Required(ErrorMessageResourceType = (typeof(MyCard.Web.Resources.ErrorMessages)), ErrorMessageResourceName = "ConfirmPasswordRequired")]
        [Display(Name = "ConfirmPassword", ResourceType = typeof(MyCard.Web.Resources.CaptionsAll))]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessageResourceType = (typeof(MyCard.Web.Resources.ErrorMessages)), ErrorMessageResourceName = "ComparePasswordErrorMessage")]
        public string ConfirmPassword { get; set; }
    }
}
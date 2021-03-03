using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace MyCard.Web.Models
{
    //public class EmailDomainValidator : ValidationAttribute, IClientValidatable
    //{
    //    protected override ValidationResult
    //            IsValid(object value, ValidationContext validationContext)
    //    {
    //        var model = (Models.RegistrationViewModel)validationContext.ObjectInstance;
    //        if(model.EmailID != null && model.DomainName != null)
    //        {
    //            string EmailID = model.EmailID.ToString();
    //            string DomainName = model.DomainName.ToString();
    //            try
    //            {
    //                MailAddress addr = new MailAddress(EmailID);
    //                string username = addr.User;
    //                string emailhost = addr.Host;

    //                int doubleSlashesIndex = model.DomainName.IndexOf("://");
    //                var start = doubleSlashesIndex != -1 ? doubleSlashesIndex + "://".Length : 0;
    //                var end = model.DomainName.IndexOf("/", start);
    //                if (end == -1)
    //                    end = model.DomainName.Length;

    //                string trimmed = model.DomainName.Substring(start, end - start);
    //                if (trimmed.StartsWith("www."))
    //                    trimmed = trimmed.Substring("www.".Length);

    //                string domainhost = trimmed;

    //                //Uri myUri = new Uri(DomainName);
    //                //string domainhost = myUri.Host;

    //                if (emailhost != domainhost)
    //                {
    //                    return new ValidationResult
    //                        (MyCard.Web.Resources.ErrorMessages.SameDomainName);
    //                }
    //                else
    //                {
    //                    return ValidationResult.Success;
    //                }
    //            }
    //            catch
    //            {
    //                return null;
    //            }
                
    //        }
    //        else
    //        {
    //            return null;
    //        }
            
    //    }

    //    public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
    //    {
    //        ModelClientValidationRule rule = new ModelClientValidationRule();
    //        rule.ErrorMessage = FormatErrorMessage(metadata.GetDisplayName());
    //        rule.ValidationType = "samedomainname";
    //        yield return rule;
    //    }


    //}

    public class RegistrationViewModel
    {
        //[Required(ErrorMessageResourceType = (typeof(MyCard.Web.Resources.ErrorMessages)), ErrorMessageResourceName = "CompanyNameRequired")]
        //[RegularExpression("^([a-zA-Z0-9 .&'-]+)$", ErrorMessageResourceType = typeof(Resources.ErrorMessages), ErrorMessageResourceName = "InvalidCompanyName")]
        [Display(Name = "CompanyName", ResourceType = typeof(MyCard.Web.Resources.CaptionsAll))]
        public string Name { get; set; }
        //[Required(ErrorMessageResourceType = (typeof(MyCard.Web.Resources.ErrorMessages)), ErrorMessageResourceName = "BusinessTitleRequired")]
        //[RegularExpression("^([a-zA-Z0-9 .&'-]+)$", ErrorMessageResourceType = typeof(Resources.ErrorMessages), ErrorMessageResourceName = "InvalidBusinessTitle")]
        [Display(Name = "BusinessTitle", ResourceType = typeof(MyCard.Web.Resources.CaptionsAll))]
        public string BusinessTitle { get; set; }
        //[Required(ErrorMessageResourceType = (typeof(MyCard.Web.Resources.ErrorMessages)), ErrorMessageResourceName = "EmailRequired")]
        [EmailAddress(ErrorMessageResourceType = (typeof(MyCard.Web.Resources.ErrorMessages)), ErrorMessageResourceName = "EmailValid")]
        [Display(Name = "EmailID", ResourceType = typeof(MyCard.Web.Resources.CaptionsAll))]
        //[EmailDomainValidator]
        public string EmailID { get; set; }
        //[Required(ErrorMessageResourceType = (typeof(MyCard.Web.Resources.ErrorMessages)), ErrorMessageResourceName = "NameRequired")]
        //[RegularExpression("^([a-zA-Z0-9 .&'-]+)$", ErrorMessage = "InvalidName")]
        [Display(Name = "CustomerName", ResourceType = typeof(MyCard.Web.Resources.CaptionsAll))]
        public string CustomerName { get; set; }
        //[Required(ErrorMessageResourceType = (typeof(MyCard.Web.Resources.ErrorMessages)), ErrorMessageResourceName = "DomainNameRequired")]
        [Display(Name = "DomainName", ResourceType = typeof(MyCard.Web.Resources.CaptionsAll))]
        //[Url(ErrorMessageResourceType = (typeof(MyCard.Web.Resources.ErrorMessages)), ErrorMessageResourceName = "DomainValid")]
        //[RegularExpression("/(http(s)?:\\)?([\\w-]+\\.)+[\\w-]+[.com|.in|.org]+(\\[\\?%&=]*)?/", ErrorMessage = "DomainValid")]
        //[RegularExpression("(?:(?:http|https):\\/\\/)?([-a-zA-Z0-9.]{2,256}\\.[a-z]{2,4})\b(?:\\/[-a-zA-Z0-9@:%_\\+.~#?&//=]*)?", ErrorMessage = "Domain Valid")]
        //[RegularExpression(@"((www\.|(http|https|ftp|news|file|)+\:\/\/)?[&#95;.a-z0-9-]+\.[a-z0-9\/&#95;:@=.+?,##%&~-]*[^.|\'|\# |!|\(|?|,| |>|<|;|\)])", ErrorMessageResourceType = typeof(Resources.ErrorMessages), ErrorMessageResourceName = "DomainValid")]
        public string DomainName { get; set; }
        public string Industry { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string CustomerType { get; set; }
        public string ServerIP { get; set; }
        public int AccountType { get; set; }
    }

    
}
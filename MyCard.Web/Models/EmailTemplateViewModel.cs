using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyCard.Web.Models
{
    public class EmailTemplateViewModel
    {
        public int ID
        {
            get;
            set;
        }
        [AllowHtml]
        public string EmailTemplate
        {
            get;
            set;
        }
        public string Subject
        {
            get;
            set;
        }
        public IEnumerable<SelectListItem> EmailTemplates
        {
            get;
            set;
        }
    }
}
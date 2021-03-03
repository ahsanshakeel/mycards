using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyCard.Web.Models
{
    public class ExportViewModel
    {
        public string SearchString { get; set; }
        public DateTime? DateFrom {get;set;}
        public DateTime? DateTo { get; set; } 
    }
}
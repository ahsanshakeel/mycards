using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyCard.Web.Models
{
    public class EmployeeViewModel
    {
        public int ID { get; set; }
        [Display(Name = "EmployeeName", ResourceType = typeof(MyCard.Web.Resources.CaptionsAll))]
        public string EmployeeName { get; set; }
        [Display(Name = "EmailID", ResourceType = typeof(MyCard.Web.Resources.CaptionsAll))]
        public string EmailID { get; set; }
        [Display(Name = "Status", ResourceType = typeof(MyCard.Web.Resources.CaptionsAll))]
        public string Status { get; set; }
        public string JoiningDate { get; set; }

    }
}
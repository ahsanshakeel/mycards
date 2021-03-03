using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyCard.Web.Models
{
    public class POListViewModel
    {
        [Display(Name = "ID", ResourceType = typeof(MyCard.Web.Resources.CaptionsAll))]
        public int ID { get; set; }
        [Display(Name = "OrderNumber", ResourceType = typeof(MyCard.Web.Resources.CaptionsAll))]
        public string OrderNumber { get; set; }
        [Display(Name = "PODate", ResourceType = typeof(MyCard.Web.Resources.CaptionsAll))]
        public string PODate { get; set; }
        [Display(Name = "Quantity", ResourceType = typeof(MyCard.Web.Resources.CaptionsAll))]
        public int Quantity { get; set; }
        [Display(Name = "OrderTotal", ResourceType = typeof(MyCard.Web.Resources.CaptionsAll))]
        public decimal OrderTotal { get; set; }
        [Display(Name = "OrderFile", ResourceType = typeof(MyCard.Web.Resources.CaptionsAll))]
        public byte[] OrderFile { get; set; }
        [Display(Name = "Approved", ResourceType = typeof(MyCard.Web.Resources.CaptionsAll))]
        public int Approved { get; set; }
        [Display(Name = "FileName", ResourceType = typeof(MyCard.Web.Resources.CaptionsAll))]
        public string FileName { get; set; }
        [Display(Name = "ContentType", ResourceType = typeof(MyCard.Web.Resources.CaptionsAll))]
        public string ContentType { get; set; }
        [Display(Name = "CompanyName", ResourceType = typeof(MyCard.Web.Resources.CaptionsAll))]
        public string CompanyName { get; set; }
        [Display(Name = "Email", ResourceType = typeof(MyCard.Web.Resources.CaptionsAll))]
        public string Email { get; set; }
        [Display(Name = "SendInvoice", ResourceType = typeof(MyCard.Web.Resources.CaptionsAll))]
        public string SendInvoice { get; set; }
        public string RejectReason { get; set; }
        public string ApprovalDate { get; set; }
        public string ExpiryDate { get; set; }
        public int LicenseId { get; set; }
        public string LicenseTypeName { get; set; }
    }
}
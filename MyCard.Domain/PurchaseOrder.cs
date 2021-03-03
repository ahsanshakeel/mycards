using MyCard.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCard.Domain
{
    [Table("purchase_order")]
    public class PurchaseOrder : EntityBase
    {
        [Column("OrderNumber")]
        public string OrderNumber { get; set; }
        [Column("LicenseId")]
        public int LicenseId { get; set; }
        [Column("CompanyId")]
        public int CompanyId { get; set; }
        [Column("LicensesOrdered")]
        public int LicensesOrdered { get; set; }
        [Column("OrderTotal")]
        public decimal OrderTotal { get; set; }
        [Column("OrderFile")]
        public byte[] OrderFile { get; set; }
        [Column("TermsAccepted")]
        public bool TermsAccepted { get; set; }
        [Column("BillingAddress")]
        public string BillingAddress { get; set; }
        [Column("Approved")]
        public int Approved { get; set; }
        [Column("file_name")]
        public string FileName { get; set; }
        [Column("content_type")]
        public string ContentType { get; set; }
        [Column("send_invoice")]
        public bool SendInvoice { get; set; }
        [Column("reject_reason")]
        public string RejectReason { get; set; }
        [Column("approval_date")]
        public DateTime ApprovalDate { get; set; }
    }
}

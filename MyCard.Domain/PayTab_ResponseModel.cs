using MyCard.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCard.Domain
{
    [Table("paytaborder")]
    public  class PayTab_ResponseModel : EntityBase
    {
        [Column("tran_ref")]
        public string tran_ref { get; set; }
        [Column("tran_type")]
        public string tran_type { get; set; }

        [Column("cart_id")]
        public string cart_id { get; set; }

        [Column("cart_description")]
        public string cart_description { get; set; }
        [Column("cart_currency")]
        public string cart_currency { get; set; }
        [Column("cart_amount")]
        public string cart_amount { get; set; }
        [Column("callback")]
        public string callback { get; set; }

        [Column("ReturnPage")]
        public string ReturnPage { get; set; }
        [Column("redirect_url")]
        public string redirect_url { get; set; }
        [Column("User_Email")]
        public string User_Email { get; set; }

        [Column("Company_Name")]
        public string Company_Name { get; set; }
        [Column("customer_Name")]
        public string customer_Name { get; set; }
        [Column("User_ID")]
        public string User_ID { get; set; }

        [Column("Company_ID")]
        public int Company_ID { get; set; }
        [Column("PurchaseOrder_Id")]
        public int PurchaseOrder_Id { get; set; }
        [Column("Payment_Date")]
        public DateTime Payment_Date { get; set; }

    }
}

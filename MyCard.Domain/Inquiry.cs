using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyCard.Core;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCard.Domain
{
    [Table("inquiries")]
    public class Inquiry : EntityBase
    {
        [Column("customer_name")]
        public string CustomerName { get; set; }
        [Column("email")]
        public string Email { get; set; }
        [Column("business_name")]
        public string BusinessName { get; set; }
    }
}

using System;
using MyCard.Core;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCard.Domain
{
    [Table("company_contacts")]
    public class CompanyContact : EntityBase
    {
        [Column("company_id")]
        public int CompanyId { get; set; }
        [Column("user_id")]
        public int UserId { get; set; }
        [Column("contact_id")]
        public int ContactId { get; set; }
        [Column("ext")]
        public string Ext { get; set; }

        [ForeignKey("CompanyId")]
        public Company Company { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        [ForeignKey("ContactId")]
        public Contact Contact { get; set; }
    }
}

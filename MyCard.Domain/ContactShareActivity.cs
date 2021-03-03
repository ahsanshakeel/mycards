using System;
using MyCard.Core;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCard.Domain
{
    [Table("contacts_share_activity")]
    public class ContactShareActivity : EntityBase
    {
        [Column("contact_id")]
        public int ContactId { get; set; }
        [Column("sender_id")]
        public int SenderId { get; set; }
        [Column("receiver_id")]
        public int ReceiverId { get; set; }
        [Column("action_performed")]
        public string ActionPerformed { get; set; }
        [Column("ip")]
        public string Ip { get; set; }
        [Column("status")]
        public string Status { get; set; }
        [ForeignKey("ContactId")]
        public Contact Contact { get; set; }


    }
}

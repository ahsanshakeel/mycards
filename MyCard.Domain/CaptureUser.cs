using System;
using MyCard.Core;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCard.Domain
{
    [Table("capture_users")]
    public class CaptureUser : EntityBase
    {
        [Column("user_id")]
        public int UserId { get; set; }
        [Column("first_name")]
        public string FirstName { get; set; }
        [Column("last_name")]
        public string LastName { get; set; }
        [Column("email")]
        public string Email { get; set; }
        [Column("phone")]
        public string Phone { get; set; }
        [Column("card_front")]
        public string CardFront { get; set; }
        [Column("card_back")]
        public string CardBack { get; set; }
        [Column("designation")]
        public string Designation { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

    }
}

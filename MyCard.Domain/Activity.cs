using MyCard.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCard.Domain
{
    [Table("activity")]
    public class Activity : EntityBase
    {
        [Column("action_id")]
        public string ActionId { get; set; }
        [Column("activity_id")]
        public string ActivityId { get; set; }
        [Column("ip")]
        public string IP { get; set; }
        [Column("user_id")]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }
        
    }
}

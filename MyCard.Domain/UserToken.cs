using System;
using MyCard.Core;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCard.Domain
{
    [Table("user_tokens")]
    public class UserToken : EntityBase
    {
        [Column("user_id")]
        public int UserId { get; set; }
        [Column("token")]
        public string Token { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}

using System;
using MyCard.Core;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCard.Domain
{
    [Table("password_history")]
    public class PasswordHistory : EntityBase
    {
        [Column("password")]
        public string Password { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }
    }
}

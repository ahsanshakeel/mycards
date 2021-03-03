using System;
using MyCard.Core;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCard.Domain
{
    [Table("password_resets")]
    public class PasswordReset : EntityBase
    {
        [Column("email")]
        public string Email { get; set; }
        [Column("token")]
        public string Token { get; set; }
    }
}

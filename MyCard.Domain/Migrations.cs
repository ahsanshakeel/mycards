using System;
using MyCard.Core;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCard.Domain
{
    [Table("migrations")]
    public class Migrations : EntityBase
    {
        [Column("migration")]
        public string Migration { get; set; }
        [Column("batch")]
        public string Batch { get; set; }
    }
}

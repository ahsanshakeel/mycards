using MyCard.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCard.Domain
{
    [Table("industry")]
    public class Industry : EntityBase
    {
        [Column("industry_name")]
        public string IndustryName { get; set; }
        
    }
}

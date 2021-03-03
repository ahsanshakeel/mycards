using MyCard.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCard.Domain
{
    [Table("license_types")]
    public class LicenseType : EntityBase
    {
        [Column("license_type_name")]
        public string LicenseTypeName { get; set; }
        [Column("from")]
        public int? From { get; set; }
        [Column("to")]
        public int? To { get; set; }
        [Column("price")]
        public decimal Price { get; set; }
        
    }
}

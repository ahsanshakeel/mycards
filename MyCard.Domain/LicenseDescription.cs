using MyCard.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCard.Domain
{
    [Table("license_descriptions")]
    public class LicenseDescription : EntityBase
    {
        [Column("license_name")]
        public string LicenseName { get; set; }
        [Column("description")]
        public string Description { get; set; }

    }
}

using MyCard.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCard.Domain
{
    [Table("cities")]
    public class City : EntityBase
    {
        [Column("city_name")]
        public string CityName { get; set; }
        [Column("country_name")]
        public string CountryName { get; set; }

    }
}

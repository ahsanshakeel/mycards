using System;
using MyCard.Core;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCard.Domain
{
    [Table("card_design")]
    public class CardDesign : EntityBase
    {
        [Column("uploader_id")]
        public int UploaderId { get; set; }
        [Column("card")]
        public string Card { get; set; }
    }
}

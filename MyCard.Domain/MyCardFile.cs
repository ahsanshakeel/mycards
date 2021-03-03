using MyCard.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCard.Domain
{
    [Table("mycard_files")]
    public class MyCardFile : EntityBase
    {
        [Column("filename")]
        public string FileName { get; set; }
        [Column("file_content")]
        public byte[] FileContent { get; set; }

    }
}

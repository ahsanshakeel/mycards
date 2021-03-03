using MyCard.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCard.Domain
{
    [Table("emailtemplate")]
    public class EmailTemplate : EntityBase
    {
        [Column("name")]
        public string Name { get; set; }
        [Column("template")]
        public string Template { get; set; }
        [Column("subject")]
        public string Subject { get; set; }
        [Column("template_id")]
        public int TemplateId { get; set; }
    }
}

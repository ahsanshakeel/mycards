using System;
using MyCard.Core;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCard.Domain
{
    [Table("companies_card_template")]
    public class CompanyCardTemplate : EntityBase
    {
        [Column("company_id")]
        public int CompanyId { get; set; }
        [Column("card_json")]
        public string CardJson { get; set; }
        [Column("card_side")]
        public int CardSide { get; set; }
        [Column("template_html")]
        public string TemplateHtml { get; set; }

        [ForeignKey("CompanyId")]
        public Company Company { get; set; }
    }
}

using System;
using MyCard.Core;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCard.Domain
{
    [Table("contacts")]
    public class Contact : EntityBase
    {
        [Column("displayName")]
        public string DisplayName { get; set; }
        [Column("first_name")]
        public string FirstName { get; set; }
        [Column("last_name")]
        public string LastName { get; set; }
        [Column("email")]
        public string Email { get; set; }
        [Column("phone")]
        public string Phone { get; set; }
        [Column("card_front")]
        public string CardFront { get; set; }
        [Column("card_back")]
        public string CardBack { get; set; }
        [Column("designation")]
        public string Designation { get; set; }
        [Column("mobile")]
        public string Mobile { get; set; }
        [Column("fax")]
        public string Fax { get; set; }
        [Column("zip")]
        public string Zip { get; set; }
        [Column("address")]
        public string Address { get; set; }
        [Column("website")]
        public string Website { get; set; }

        public IList<CompanyContact> CompanyContacts { get; set; }
        public IList<ContactShareActivity> ContactShareActivities { get; set; }
        
    }
}

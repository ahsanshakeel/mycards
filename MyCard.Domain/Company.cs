using MyCard.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCard.Domain
{
    [Table("companies")]
    public class Company : EntityBase
    {
        [Column("domain")]
        public string Domain { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("email")]
        public string Email { get; set; }
        [Column("phone")]
        public string Phone { get; set; }
        [Column("address")]
        public string Address { get; set; }
        [Column("city")]
        public string City { get; set; }
        [Column("state")]
        public string State { get; set; }
        [Column("zip")]
        public string Zip { get; set; }
        [Column("logo")]
        public byte[] Logo { get; set; }
        [Column("business_title")]
        public string BusinessTitle { get; set; }
        [Column("logo_file_name")]
        public string LogoFileName { get; set; }
        [Column("logo_content_type")]
        public string LogoContentType { get; set; }
        [Column("customer_name")]
        public string CustomerName { get; set; }
        [Column("industry")]
        public string Industry { get; set; }
        [Column("country")]
        public string Country { get; set; }
        [Column("customer_type")]
        public string CustomerType { get; set; }
        [Column("approved")]
        public string Approved { get; set; }
        [Column("server_ip")]
        public string ServerIp { get; set; }
        [Column("account_type")]
        public int AccountType { get; set; }
        [Column("cms_access_key")]
        public string CmsAccessKey { get; set; }
        [Column("cp_password")]
        public string CpPassword { get; set; }
        [Column("pass_reset_code")]
        public string PassResetCode { get; set; }

        public IList<User> Users { get; set; }
        public IList<CompanyCardTemplate> CompanyCardTemplates { get; set; }
        public IList<CompanyContact> CompanyContacts { get; set; }
        public IList<SyncedUser> SyncedUsers { get; set; }
                
    }
}

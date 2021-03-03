using System;
using MyCard.Core;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCard.Domain
{
    [Table("users")]
    public class User : EntityBase
    {
        [Column("name")]
        public string Name { get; set; }
        [Column("email")]
        public string Email { get; set; }
        [Column("password")]
        public string Password { get; set; }
        [Column("active")]
        public int Active { get; set; }
        [Column("remember_token")]
        public string RememberToken { get; set; }
        [Column("company_id")]
        public int CompanyId { get; set; }
        [Column("outlook_profile_pic")]
        public string OutlookProfilePic { get; set; }
        [Column("coordinates")]
        public string Coordinates { get; set; }

        [ForeignKey("CompanyId")]
        public Company Company { get; set; }

        public IList<Activity> Activities { get; set; }
        public IList<CaptureUser> CaptureUsers { get; set; }
        public IList<CompanyContact> CompanyContacts { get; set; }
        public IList<SyncedUser> SyncedUsers { get; set; }
        public IList<UserRole> UserRoles { get; set; }
        public IList<UserToken> UserTokens { get; set; }
    }
}

using System;
using MyCard.Core;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCard.Domain
{
    [Table("synced_users")]
    public class SyncedUser : EntityBase
    {
        [Column("user_id")]
        public int UserId { get; set; }
        [Column("friend_id")]
        public int FriendId { get; set; }
        [Column("notes")]
        public string Notes { get; set; }
        [Column("company_id")]
        public int CompanyId { get; set; }
        [Column("unsynced")]
        public int Unsynced { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        [ForeignKey("CompanyId")]
        public Company Company { get; set; }
    }
}

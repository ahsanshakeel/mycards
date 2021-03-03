using System;
using MyCard.Core;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCard.Domain
{
    [Table("roles_permissions")]
    public class RolePermission : EntityBase
    {
        [Column("role_id")]
        public int RoleId { get; set; }
        [Column("permission_id")]
        public int PermissionId { get; set; }

        [ForeignKey("RoleId")]
        public Role Role { get; set; }

        [ForeignKey("PermissionId")]
        public Permission Permission { get; set; }
    }
}

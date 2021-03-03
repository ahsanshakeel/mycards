using System;
using MyCard.Core;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyCard.Domain
{
    [Table("roles")]
    public class Role : EntityBase
    {
        [Column("name")]
        public string Name { get; set; }

        public IList<RolePermission> RolePermissions { get; set; }
        public IList<UserRole> UserRoles { get; set; }
        
    }
}

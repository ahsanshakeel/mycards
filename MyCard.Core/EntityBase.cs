using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCard.Core
{
    public abstract class EntityBase
    {
        public EntityBase()
        {
            this.CreationDate = DateTime.UtcNow;
            this.LastUpdateDate = DateTime.UtcNow;
        }
        //private int? requestedHashCode;
        private int id;
        [Column("id")]
        public virtual int Id
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
            }
        }
        [DataType(DataType.DateTime)]
        [Column("created_at")]
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Get or set the Date of LastUpdate 
        /// </summary>
        [DataType(DataType.DateTime)]
        [Column("updated_at")]
        public DateTime LastUpdateDate { get; set; }
        

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is EntityBase))
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            EntityBase item = (EntityBase)obj;

            return item.Id == this.Id;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();

        }
    }
}

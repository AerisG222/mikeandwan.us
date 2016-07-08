using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Identity
{
    [Table("role", Schema = "maw")]
    public partial class Role
    {
        public Role()
        {
            UserRole = new HashSet<UserRole>();
        }

        [Column("id")]
        public short Id { get; set; }
        [Required]
        [Column("name", TypeName = "varchar")]
        [MaxLength(30)]
        public string Name { get; set; }
        [Column("description", TypeName = "varchar")]
        [MaxLength(255)]
        public string Description { get; set; }

        [InverseProperty("Role")]
        public virtual ICollection<UserRole> UserRole { get; set; }
    }
}

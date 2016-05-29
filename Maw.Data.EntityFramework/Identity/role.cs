using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Identity
{
    [Table("role", Schema = "maw")]
    public partial class role
    {
        public role()
        {
            user_role = new HashSet<user_role>();
        }

        public short id { get; set; }
        [MaxLength(255)]
        public string description { get; set; }
        [Required]
        [MaxLength(30)]
        public string name { get; set; }

        [InverseProperty("role")]
        public virtual ICollection<user_role> user_role { get; set; }
    }
}

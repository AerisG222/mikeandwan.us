using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Identity
{
    [Table("state", Schema = "maw")]
    public partial class state
    {
        public state()
        {
            user = new HashSet<user>();
        }

        public short id { get; set; }
        [Required]
        [MaxLength(2)]
        public string code { get; set; }
        [Required]
        [MaxLength(30)]
        public string name { get; set; }

        [InverseProperty("state")]
        public virtual ICollection<user> user { get; set; }
    }
}

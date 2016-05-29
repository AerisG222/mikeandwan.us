using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Identity
{
    [Table("country", Schema = "maw")]
    public partial class country
    {
        public country()
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

        [InverseProperty("country")]
        public virtual ICollection<user> user { get; set; }
    }
}

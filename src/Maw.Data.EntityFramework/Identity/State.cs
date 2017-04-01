using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Identity
{
    [Table("state", Schema = "maw")]
    public partial class State
    {
        public State()
        {
            User = new HashSet<User>();
        }

        [Column("id")]
        public short Id { get; set; }
        [Required]
        [Column("code", TypeName = "bpchar")]
        [MaxLength(2)]
        public string Code { get; set; }
        [Required]
        [Column("name", TypeName = "varchar")]
        [MaxLength(30)]
        public string Name { get; set; }

        [InverseProperty("State")]
        public virtual ICollection<User> User { get; set; }
    }
}

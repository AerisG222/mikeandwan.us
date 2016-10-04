using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Photos
{
    [Table("make", Schema = "photo")]
    public partial class Make
    {
        public Make()
        {
            Photo = new HashSet<Photo>();
        }

        [Column("id")]
        public short Id { get; set; }
        [Required]
        [Column("name", TypeName = "varchar")]
        [MaxLength(50)]
        public string Name { get; set; }

        [InverseProperty("Make")]
        public virtual ICollection<Photo> Photo { get; set; }
    }
}

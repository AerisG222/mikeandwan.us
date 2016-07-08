using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Photos
{
    [Table("lens", Schema = "photo")]
    public partial class Lens
    {
        public Lens()
        {
            Photo = new HashSet<Photo>();
        }

        [Column("id")]
        public short Id { get; set; }
        [Required]
        [Column("name", TypeName = "varchar")]
        [MaxLength(100)]
        public string Name { get; set; }

        [InverseProperty("Lens")]
        public virtual ICollection<Photo> Photo { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Photos
{
    [Table("colorspace", Schema = "photo")]
    public partial class Colorspace
    {
        public Colorspace()
        {
            Photo = new HashSet<Photo>();
        }

        [Column("id")]
        public short Id { get; set; }
        [Required]
        [Column("name", TypeName = "varchar")]
        [MaxLength(15)]
        public string Name { get; set; }

        [InverseProperty("Colorspace")]
        public virtual ICollection<Photo> Photo { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Photos
{
    [Table("flash", Schema = "photo")]
    public partial class Flash
    {
        public Flash()
        {
            Photo = new HashSet<Photo>();
        }

        [Column("id")]
        public int Id { get; set; }
        [Required]
        [Column("name", TypeName = "varchar")]
        [MaxLength(60)]
        public string Name { get; set; }

        [InverseProperty("Flash")]
        public virtual ICollection<Photo> Photo { get; set; }
    }
}

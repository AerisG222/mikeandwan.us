using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Photos
{
    [Table("light_source", Schema = "photo")]
    public partial class LightSource
    {
        public LightSource()
        {
            Photo = new HashSet<Photo>();
        }

        [Column("id")]
        public int Id { get; set; }
        [Required]
        [Column("name", TypeName = "varchar")]
        [MaxLength(30)]
        public string Name { get; set; }

        [InverseProperty("LightSource")]
        public virtual ICollection<Photo> Photo { get; set; }
    }
}

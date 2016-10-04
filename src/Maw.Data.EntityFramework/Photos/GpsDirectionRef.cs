using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Photos
{
    [Table("gps_direction_ref", Schema = "photo")]
    public partial class GpsDirectionRef
    {
        public GpsDirectionRef()
        {
            Photo = new HashSet<Photo>();
        }

        [Column("id", TypeName = "varchar")]
        [MaxLength(2)]
        public string Id { get; set; }
        [Required]
        [Column("name", TypeName = "varchar")]
        [MaxLength(20)]
        public string Name { get; set; }

        [InverseProperty("GpsDirectionRef")]
        public virtual ICollection<Photo> Photo { get; set; }
    }
}

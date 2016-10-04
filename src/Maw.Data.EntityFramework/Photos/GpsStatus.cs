using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Photos
{
    [Table("gps_status", Schema = "photo")]
    public partial class GpsStatus
    {
        public GpsStatus()
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

        [InverseProperty("GpsStatus")]
        public virtual ICollection<Photo> Photo { get; set; }
    }
}

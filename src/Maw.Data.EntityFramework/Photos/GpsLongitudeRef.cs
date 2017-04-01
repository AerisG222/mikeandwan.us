using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Photos
{
    [Table("gps_longitude_ref", Schema = "photo")]
    public partial class GpsLongitudeRef
    {
        public GpsLongitudeRef()
        {
            Photo = new HashSet<Photo>();
        }

        [Column("id", TypeName = "varchar")]
        [MaxLength(2)]
        public string Id { get; set; }
        [Required]
        [Column("name", TypeName = "varchar")]
        [MaxLength(10)]
        public string Name { get; set; }

        [InverseProperty("GpsLongitudeRef")]
        public virtual ICollection<Photo> Photo { get; set; }
    }
}

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Photos
{
    [Table("gps_altitude_ref", Schema = "photo")]
    public partial class GpsAltitudeRef
    {
        public GpsAltitudeRef()
        {
            Photo = new HashSet<Photo>();
        }

        [Column("id")]
        public short Id { get; set; }
        [Required]
        [Column("name", TypeName = "varchar")]
        [MaxLength(20)]
        public string Name { get; set; }

        [InverseProperty("GpsAltitudeRef")]
        public virtual ICollection<Photo> Photo { get; set; }
    }
}

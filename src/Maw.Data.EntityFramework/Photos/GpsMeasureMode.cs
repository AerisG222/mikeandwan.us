using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Photos
{
    [Table("gps_measure_mode", Schema = "photo")]
    public partial class GpsMeasureMode
    {
        public GpsMeasureMode()
        {
            Photo = new HashSet<Photo>();
        }

        [Column("id", TypeName = "varchar")]
        [MaxLength(2)]
        public string Id { get; set; }
        [Required]
        [Column("name", TypeName = "varchar")]
        [MaxLength(30)]
        public string Name { get; set; }

        [InverseProperty("GpsMeasureMode")]
        public virtual ICollection<Photo> Photo { get; set; }
    }
}

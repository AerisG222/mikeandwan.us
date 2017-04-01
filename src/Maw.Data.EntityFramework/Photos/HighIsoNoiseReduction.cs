using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Photos
{
    [Table("high_iso_noise_reduction", Schema = "photo")]
    public partial class HighIsoNoiseReduction
    {
        public HighIsoNoiseReduction()
        {
            Photo = new HashSet<Photo>();
        }

        [Column("id")]
        public short Id { get; set; }
        [Required]
        [Column("name", TypeName = "varchar")]
        [MaxLength(20)]
        public string Name { get; set; }

        [InverseProperty("HighIsoNoiseReduction")]
        public virtual ICollection<Photo> Photo { get; set; }
    }
}

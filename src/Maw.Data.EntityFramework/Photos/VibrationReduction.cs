using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Photos
{
    [Table("vibration_reduction", Schema = "photo")]
    public partial class VibrationReduction
    {
        public VibrationReduction()
        {
            Photo = new HashSet<Photo>();
        }

        [Column("id")]
        public short Id { get; set; }
        [Required]
        [Column("name", TypeName = "varchar")]
        [MaxLength(10)]
        public string Name { get; set; }

        [InverseProperty("VibrationReduction")]
        public virtual ICollection<Photo> Photo { get; set; }
    }
}

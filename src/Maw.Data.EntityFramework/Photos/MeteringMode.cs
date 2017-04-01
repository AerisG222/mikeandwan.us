using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Photos
{
    [Table("metering_mode", Schema = "photo")]
    public partial class MeteringMode
    {
        public MeteringMode()
        {
            Photo = new HashSet<Photo>();
        }

        [Column("id")]
        public int Id { get; set; }
        [Required]
        [Column("name", TypeName = "varchar")]
        [MaxLength(50)]
        public string Name { get; set; }

        [InverseProperty("MeteringMode")]
        public virtual ICollection<Photo> Photo { get; set; }
    }
}

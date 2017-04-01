using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Photos
{
    [Table("active_d_lighting", Schema = "photo")]
    public partial class ActiveDLighting
    {
        public ActiveDLighting()
        {
            Photo = new HashSet<Photo>();
        }

        [Column("id")]
        public short Id { get; set; }
        [Required]
        [Column("name", TypeName = "varchar")]
        [MaxLength(20)]
        public string Name { get; set; }

        [InverseProperty("ActiveDLighting")]
        public virtual ICollection<Photo> Photo { get; set; }
    }
}

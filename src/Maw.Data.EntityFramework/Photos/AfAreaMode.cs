using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Photos
{
    [Table("af_area_mode", Schema = "photo")]
    public partial class AfAreaMode
    {
        public AfAreaMode()
        {
            Photo = new HashSet<Photo>();
        }

        [Column("id")]
        public short Id { get; set; }
        [Required]
        [Column("name", TypeName = "varchar")]
        [MaxLength(40)]
        public string Name { get; set; }

        [InverseProperty("AfAreaMode")]
        public virtual ICollection<Photo> Photo { get; set; }
    }
}

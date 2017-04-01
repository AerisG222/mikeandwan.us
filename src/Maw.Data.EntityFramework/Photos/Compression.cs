using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Photos
{
    [Table("compression", Schema = "photo")]
    public partial class Compression
    {
        public Compression()
        {
            Photo = new HashSet<Photo>();
        }

        [Column("id")]
        public int Id { get; set; }
        [Required]
        [Column("name", TypeName = "varchar")]
        [MaxLength(80)]
        public string Name { get; set; }

        [InverseProperty("Compression")]
        public virtual ICollection<Photo> Photo { get; set; }
    }
}

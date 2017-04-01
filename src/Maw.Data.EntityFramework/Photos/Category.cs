using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Photos
{
    [Table("category", Schema = "photo")]
    public partial class Category
    {
        public Category()
        {
            Photo = new HashSet<Photo>();
        }

        [Column("id")]
        public short Id { get; set; }
        [Column("year")]
        public short Year { get; set; }
        [Column("is_private")]
        public bool IsPrivate { get; set; }
        [Required]
        [Column("name", TypeName = "varchar")]
        [MaxLength(50)]
        public string Name { get; set; }
        [Column("teaser_photo_width")]
        public short? TeaserPhotoWidth { get; set; }
        [Column("teaser_photo_height")]
        public short? TeaserPhotoHeight { get; set; }
        [Column("teaser_photo_path", TypeName = "varchar")]
        [MaxLength(255)]
        public string TeaserPhotoPath { get; set; }

        [InverseProperty("Category")]
        public virtual ICollection<Photo> Photo { get; set; }
    }
}

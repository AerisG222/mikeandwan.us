using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Videos
{
    [Table("category", Schema = "video")]
    public partial class Category
    {
        public Category()
        {
            Video = new HashSet<Video>();
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
        [Column("teaser_image_path", TypeName = "varchar")]
        [MaxLength(255)]
        public string TeaserImagePath { get; set; }
        [Column("teaser_image_width")]
        public short? TeaserImageWidth { get; set; }
        [Column("teaser_image_height")]
        public short? TeaserImageHeight { get; set; }

        [InverseProperty("Category")]
        public virtual ICollection<Video> Video { get; set; }
    }
}

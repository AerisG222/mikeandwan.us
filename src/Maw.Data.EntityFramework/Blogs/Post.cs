using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Blogs
{
    [Table("post", Schema = "blog")]
    public partial class Post
    {
        [Column("id")]
        public short Id { get; set; }
        [Column("blog_id")]
        public short BlogId { get; set; }
        [Required]
        [Column("title", TypeName = "varchar")]
        [MaxLength(100)]
        public string Title { get; set; }
        [Required]
        [Column("description")]
        public string Description { get; set; }
        [Column("publish_date")]
        public DateTime PublishDate { get; set; }

        [ForeignKey("BlogId")]
        [InverseProperty("Post")]
        public virtual Blog Blog { get; set; }
    }
}

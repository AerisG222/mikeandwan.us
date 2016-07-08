using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Blogs
{
    [Table("blog", Schema = "blog")]
    public partial class Blog
    {
        public Blog()
        {
            Post = new HashSet<Post>();
        }

        [Column("id")]
        public short Id { get; set; }
        [Required]
        [Column("title", TypeName = "varchar")]
        [MaxLength(50)]
        public string Title { get; set; }
        [Required]
        [Column("copyright", TypeName = "varchar")]
        [MaxLength(50)]
        public string Copyright { get; set; }
        [Required]
        [Column("description", TypeName = "varchar")]
        [MaxLength(250)]
        public string Description { get; set; }

        [InverseProperty("Blog")]
        public virtual ICollection<Post> Post { get; set; }
    }
}

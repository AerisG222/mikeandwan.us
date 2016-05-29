using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Blogs
{
    [Table("post", Schema = "blog")]
    public partial class post
    {
        public short id { get; set; }
        public short blog_id { get; set; }
        [Required]
        public string description { get; set; }
        public DateTime publish_date { get; set; }
        [Required]
        [MaxLength(100)]
        public string title { get; set; }

        [ForeignKey("blog_id")]
        [InverseProperty("post")]
        public virtual blog blog { get; set; }
    }
}

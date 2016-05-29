using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Blogs
{
    [Table("blog", Schema = "blog")]
    public partial class blog
    {
        public blog()
        {
            post = new HashSet<post>();
        }

        public short id { get; set; }
        [Required]
        [MaxLength(50)]
        public string copyright { get; set; }
        [Required]
        [MaxLength(250)]
        public string description { get; set; }
        [Required]
        [MaxLength(50)]
        public string title { get; set; }

        [InverseProperty("blog")]
        public virtual ICollection<post> post { get; set; }
    }
}

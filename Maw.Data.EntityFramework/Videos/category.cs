using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Videos
{
    [Table("category", Schema = "video")]
    public partial class category
    {
        public category()
        {
            video = new HashSet<video>();
        }

        public short id { get; set; }
        public bool is_private { get; set; }
        [Required]
        [MaxLength(50)]
        public string name { get; set; }
        public short? teaser_image_height { get; set; }
        [MaxLength(255)]
        public string teaser_image_path { get; set; }
        public short? teaser_image_width { get; set; }
        public short year { get; set; }

        [InverseProperty("category")]
        public virtual ICollection<video> video { get; set; }
    }
}

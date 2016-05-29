using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Videos
{
    [Table("video", Schema = "video")]
    public partial class video
    {
        public short id { get; set; }
        public short category_id { get; set; }
        public short duration { get; set; }
        public short full_height { get; set; }
        [Required]
        [MaxLength(255)]
        public string full_path { get; set; }
        public short full_width { get; set; }
        public bool is_private { get; set; }
        [Required]
        [MaxLength(255)]
        public string raw_path { get; set; }
        public short scaled_height { get; set; }
        [Required]
        [MaxLength(255)]
        public string scaled_path { get; set; }
        public short scaled_width { get; set; }
        public short thumb_height { get; set; }
        [Required]
        [MaxLength(255)]
        public string thumb_path { get; set; }
        public short thumb_width { get; set; }

        [ForeignKey("category_id")]
        [InverseProperty("video")]
        public virtual category category { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Videos
{
    [Table("video", Schema = "video")]
    public partial class Video
    {
        [Column("id")]
        public short Id { get; set; }
        [Column("category_id")]
        public short CategoryId { get; set; }
        [Column("is_private")]
        public bool IsPrivate { get; set; }
        [Column("duration")]
        public short Duration { get; set; }
        [Column("thumb_height")]
        public short ThumbHeight { get; set; }
        [Column("thumb_width")]
        public short ThumbWidth { get; set; }
        [Required]
        [Column("thumb_path", TypeName = "varchar")]
        [MaxLength(255)]
        public string ThumbPath { get; set; }
        [Column("scaled_height")]
        public short ScaledHeight { get; set; }
        [Column("scaled_width")]
        public short ScaledWidth { get; set; }
        [Required]
        [Column("scaled_path", TypeName = "varchar")]
        [MaxLength(255)]
        public string ScaledPath { get; set; }
        [Column("full_height")]
        public short FullHeight { get; set; }
        [Column("full_width")]
        public short FullWidth { get; set; }
        [Required]
        [Column("full_path", TypeName = "varchar")]
        [MaxLength(255)]
        public string FullPath { get; set; }
        [Required]
        [Column("raw_path", TypeName = "varchar")]
        [MaxLength(255)]
        public string RawPath { get; set; }

        [ForeignKey("CategoryId")]
        [InverseProperty("Video")]
        public virtual Category Category { get; set; }
    }
}

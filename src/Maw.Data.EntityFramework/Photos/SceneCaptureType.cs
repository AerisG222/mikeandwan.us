using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Photos
{
    [Table("scene_capture_type", Schema = "photo")]
    public partial class SceneCaptureType
    {
        public SceneCaptureType()
        {
            Photo = new HashSet<Photo>();
        }

        [Column("id")]
        public int Id { get; set; }
        [Required]
        [Column("name", TypeName = "varchar")]
        [MaxLength(50)]
        public string Name { get; set; }

        [InverseProperty("SceneCaptureType")]
        public virtual ICollection<Photo> Photo { get; set; }
    }
}

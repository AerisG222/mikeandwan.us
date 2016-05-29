using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Photos
{
    [Table("scene_capture_type", Schema = "photo")]
    public partial class scene_capture_type
    {
        public scene_capture_type()
        {
            photo = new HashSet<photo>();
        }

        public int id { get; set; }
        [Required]
        [MaxLength(50)]
        public string name { get; set; }

        [InverseProperty("scene_capture_type")]
        public virtual ICollection<photo> photo { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Photos
{
    [Table("category", Schema = "photo")]
    public partial class category
    {
        public category()
        {
            photo = new HashSet<photo>();
        }

        public short id { get; set; }
        public bool is_private { get; set; }
        [Required]
        [MaxLength(50)]
        public string name { get; set; }
        public short? teaser_photo_height { get; set; }
        [MaxLength(255)]
        public string teaser_photo_path { get; set; }
        public short? teaser_photo_width { get; set; }
        public short year { get; set; }

        [InverseProperty("category")]
        public virtual ICollection<photo> photo { get; set; }
    }
}

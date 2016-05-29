using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Photos
{
    [Table("flash_color_filter", Schema = "photo")]
    public partial class flash_color_filter
    {
        public flash_color_filter()
        {
            photo = new HashSet<photo>();
        }

        public short id { get; set; }
        [Required]
        [MaxLength(10)]
        public string name { get; set; }

        [InverseProperty("flash_color_filter")]
        public virtual ICollection<photo> photo { get; set; }
    }
}

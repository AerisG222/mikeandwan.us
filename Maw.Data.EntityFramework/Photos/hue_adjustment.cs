using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Photos
{
    [Table("hue_adjustment", Schema = "photo")]
    public partial class hue_adjustment
    {
        public hue_adjustment()
        {
            photo = new HashSet<photo>();
        }

        public short id { get; set; }
        [Required]
        [MaxLength(50)]
        public string name { get; set; }

        [InverseProperty("hue_adjustment")]
        public virtual ICollection<photo> photo { get; set; }
    }
}

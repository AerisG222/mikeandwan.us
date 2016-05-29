using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Photos
{
    [Table("raw_conversion_mode", Schema = "photo")]
    public partial class raw_conversion_mode
    {
        public raw_conversion_mode()
        {
            photo = new HashSet<photo>();
        }

        public int id { get; set; }
        [Required]
        [MaxLength(50)]
        public string name { get; set; }

        [InverseProperty("raw_conversion_mode")]
        public virtual ICollection<photo> photo { get; set; }
    }
}

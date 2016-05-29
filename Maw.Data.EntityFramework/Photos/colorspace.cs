using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Photos
{
    [Table("colorspace", Schema = "photo")]
    public partial class colorspace
    {
        public colorspace()
        {
            photo = new HashSet<photo>();
        }

        public short id { get; set; }
        [Required]
        [MaxLength(15)]
        public string name { get; set; }

        [InverseProperty("colorspace")]
        public virtual ICollection<photo> photo { get; set; }
    }
}

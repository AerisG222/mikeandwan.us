using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Photos
{
    [Table("active_d_lighting", Schema = "photo")]
    public partial class active_d_lighting
    {
        public active_d_lighting()
        {
            photo = new HashSet<photo>();
        }

        public short id { get; set; }
        [Required]
        [MaxLength(20)]
        public string name { get; set; }

        [InverseProperty("active_d_lighting")]
        public virtual ICollection<photo> photo { get; set; }
    }
}

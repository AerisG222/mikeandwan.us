using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Photos
{
    [Table("vibration_reduction", Schema = "photo")]
    public partial class vibration_reduction
    {
        public vibration_reduction()
        {
            photo = new HashSet<photo>();
        }

        public short id { get; set; }
        [Required]
        [MaxLength(10)]
        public string name { get; set; }

        [InverseProperty("vibration_reduction")]
        public virtual ICollection<photo> photo { get; set; }
    }
}

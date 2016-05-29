using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Photos
{
    [Table("high_iso_noise_reduction", Schema = "photo")]
    public partial class high_iso_noise_reduction
    {
        public high_iso_noise_reduction()
        {
            photo = new HashSet<photo>();
        }

        public short id { get; set; }
        [Required]
        [MaxLength(20)]
        public string name { get; set; }

        [InverseProperty("high_iso_noise_reduction")]
        public virtual ICollection<photo> photo { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Photos
{
    [Table("noise_reduction", Schema = "photo")]
    public partial class noise_reduction
    {
        public noise_reduction()
        {
            photo = new HashSet<photo>();
        }

        public short id { get; set; }
        [Required]
        [MaxLength(50)]
        public string name { get; set; }

        [InverseProperty("noise_reduction")]
        public virtual ICollection<photo> photo { get; set; }
    }
}

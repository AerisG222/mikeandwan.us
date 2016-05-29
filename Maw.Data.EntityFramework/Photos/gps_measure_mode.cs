using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Photos
{
    [Table("gps_measure_mode", Schema = "photo")]
    public partial class gps_measure_mode
    {
        public gps_measure_mode()
        {
            photo = new HashSet<photo>();
        }

        [MaxLength(2)]
        public string id { get; set; }
        [Required]
        [MaxLength(30)]
        public string name { get; set; }

        [InverseProperty("gps_measure_mode")]
        public virtual ICollection<photo> photo { get; set; }
    }
}

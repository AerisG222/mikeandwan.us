using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Photos
{
    [Table("gps_altitude_ref", Schema = "photo")]
    public partial class gps_altitude_ref
    {
        public gps_altitude_ref()
        {
            photo = new HashSet<photo>();
        }

        public short id { get; set; }
        [Required]
        [MaxLength(20)]
        public string name { get; set; }

        [InverseProperty("gps_altitude_ref")]
        public virtual ICollection<photo> photo { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Photos
{
    [Table("gps_direction_ref", Schema = "photo")]
    public partial class gps_direction_ref
    {
        public gps_direction_ref()
        {
            photo = new HashSet<photo>();
        }

        [MaxLength(2)]
        public string id { get; set; }
        [Required]
        [MaxLength(20)]
        public string name { get; set; }

        [InverseProperty("gps_direction_ref")]
        public virtual ICollection<photo> photo { get; set; }
    }
}

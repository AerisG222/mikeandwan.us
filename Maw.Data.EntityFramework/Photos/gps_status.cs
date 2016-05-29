using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Photos
{
    [Table("gps_status", Schema = "photo")]
    public partial class gps_status
    {
        public gps_status()
        {
            photo = new HashSet<photo>();
        }

        [MaxLength(2)]
        public string id { get; set; }
        [Required]
        [MaxLength(20)]
        public string name { get; set; }

        [InverseProperty("gps_status")]
        public virtual ICollection<photo> photo { get; set; }
    }
}

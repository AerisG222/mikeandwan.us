using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Photos
{
    [Table("gps_latitude_ref", Schema = "photo")]
    public partial class gps_latitude_ref
    {
        public gps_latitude_ref()
        {
            photo = new HashSet<photo>();
        }

        [MaxLength(2)]
        public string id { get; set; }
        [Required]
        [MaxLength(10)]
        public string name { get; set; }

        [InverseProperty("gps_latitude_ref")]
        public virtual ICollection<photo> photo { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Photos
{
    [Table("metering_mode", Schema = "photo")]
    public partial class metering_mode
    {
        public metering_mode()
        {
            photo = new HashSet<photo>();
        }

        public int id { get; set; }
        [Required]
        [MaxLength(50)]
        public string name { get; set; }

        [InverseProperty("metering_mode")]
        public virtual ICollection<photo> photo { get; set; }
    }
}

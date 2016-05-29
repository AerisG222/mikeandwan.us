using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Photos
{
    [Table("exposure_mode", Schema = "photo")]
    public partial class exposure_mode
    {
        public exposure_mode()
        {
            photo = new HashSet<photo>();
        }

        public int id { get; set; }
        [Required]
        [MaxLength(50)]
        public string name { get; set; }

        [InverseProperty("exposure_mode")]
        public virtual ICollection<photo> photo { get; set; }
    }
}

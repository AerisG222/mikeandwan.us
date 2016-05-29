using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Photos
{
    [Table("vr_mode", Schema = "photo")]
    public partial class vr_mode
    {
        public vr_mode()
        {
            photo = new HashSet<photo>();
        }

        public short id { get; set; }
        [Required]
        [MaxLength(20)]
        public string name { get; set; }

        [InverseProperty("vr_mode")]
        public virtual ICollection<photo> photo { get; set; }
    }
}

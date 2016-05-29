using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Photos
{
    [Table("focus_mode", Schema = "photo")]
    public partial class focus_mode
    {
        public focus_mode()
        {
            photo = new HashSet<photo>();
        }

        public short id { get; set; }
        [Required]
        [MaxLength(50)]
        public string name { get; set; }

        [InverseProperty("focus_mode")]
        public virtual ICollection<photo> photo { get; set; }
    }
}

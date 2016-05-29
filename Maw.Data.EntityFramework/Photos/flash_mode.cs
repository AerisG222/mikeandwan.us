using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Photos
{
    [Table("flash_mode", Schema = "photo")]
    public partial class flash_mode
    {
        public flash_mode()
        {
            photo = new HashSet<photo>();
        }

        public short id { get; set; }
        [Required]
        [MaxLength(30)]
        public string name { get; set; }

        [InverseProperty("flash_mode")]
        public virtual ICollection<photo> photo { get; set; }
    }
}

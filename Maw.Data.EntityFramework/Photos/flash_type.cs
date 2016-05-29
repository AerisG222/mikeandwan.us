using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Photos
{
    [Table("flash_type", Schema = "photo")]
    public partial class flash_type
    {
        public flash_type()
        {
            photo = new HashSet<photo>();
        }

        public short id { get; set; }
        [Required]
        [MaxLength(50)]
        public string name { get; set; }

        [InverseProperty("flash_type")]
        public virtual ICollection<photo> photo { get; set; }
    }
}

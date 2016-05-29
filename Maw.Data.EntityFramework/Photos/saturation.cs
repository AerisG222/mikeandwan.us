using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Photos
{
    [Table("saturation", Schema = "photo")]
    public partial class saturation
    {
        public saturation()
        {
            photo = new HashSet<photo>();
        }

        public short id { get; set; }
        [Required]
        [MaxLength(50)]
        public string name { get; set; }

        [InverseProperty("saturation")]
        public virtual ICollection<photo> photo { get; set; }
    }
}

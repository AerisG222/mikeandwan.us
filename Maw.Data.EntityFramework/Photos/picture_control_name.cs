using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Photos
{
    [Table("picture_control_name", Schema = "photo")]
    public partial class picture_control_name
    {
        public picture_control_name()
        {
            photo = new HashSet<photo>();
        }

        public short id { get; set; }
        [Required]
        [MaxLength(50)]
        public string name { get; set; }

        [InverseProperty("picture_control_name")]
        public virtual ICollection<photo> photo { get; set; }
    }
}

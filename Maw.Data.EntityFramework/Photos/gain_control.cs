using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Photos
{
    [Table("gain_control", Schema = "photo")]
    public partial class gain_control
    {
        public gain_control()
        {
            photo = new HashSet<photo>();
        }

        public int id { get; set; }
        [Required]
        [MaxLength(20)]
        public string name { get; set; }

        [InverseProperty("gain_control")]
        public virtual ICollection<photo> photo { get; set; }
    }
}

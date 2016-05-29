using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Photos
{
    [Table("sensing_method", Schema = "photo")]
    public partial class sensing_method
    {
        public sensing_method()
        {
            photo = new HashSet<photo>();
        }

        public int id { get; set; }
        [Required]
        [MaxLength(50)]
        public string name { get; set; }

        [InverseProperty("sensing_method")]
        public virtual ICollection<photo> photo { get; set; }
    }
}

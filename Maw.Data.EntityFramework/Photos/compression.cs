using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Photos
{
    [Table("compression", Schema = "photo")]
    public partial class compression
    {
        public compression()
        {
            photo = new HashSet<photo>();
        }

        public int id { get; set; }
        [Required]
        [MaxLength(80)]
        public string name { get; set; }

        [InverseProperty("compression")]
        public virtual ICollection<photo> photo { get; set; }
    }
}

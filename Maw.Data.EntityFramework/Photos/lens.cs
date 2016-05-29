using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Photos
{
    [Table("lens", Schema = "photo")]
    public partial class lens
    {
        public lens()
        {
            photo = new HashSet<photo>();
        }

        public short id { get; set; }
        [Required]
        [MaxLength(100)]
        public string name { get; set; }

        [InverseProperty("lens")]
        public virtual ICollection<photo> photo { get; set; }
    }
}

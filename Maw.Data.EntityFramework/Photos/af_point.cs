using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Photos
{
    [Table("af_point", Schema = "photo")]
    public partial class af_point
    {
        public af_point()
        {
            photo = new HashSet<photo>();
        }

        public short id { get; set; }
        [Required]
        [MaxLength(20)]
        public string name { get; set; }

        [InverseProperty("af_point")]
        public virtual ICollection<photo> photo { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Photos
{
    [Table("contrast", Schema = "photo")]
    public partial class contrast
    {
        public contrast()
        {
            photo = new HashSet<photo>();
        }

        public int id { get; set; }
        [Required]
        [MaxLength(10)]
        public string name { get; set; }

        [InverseProperty("contrast")]
        public virtual ICollection<photo> photo { get; set; }
    }
}

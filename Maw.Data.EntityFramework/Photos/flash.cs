using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Photos
{
    [Table("flash", Schema = "photo")]
    public partial class flash
    {
        public flash()
        {
            photo = new HashSet<photo>();
        }

        public int id { get; set; }
        [Required]
        [MaxLength(60)]
        public string name { get; set; }

        [InverseProperty("flash")]
        public virtual ICollection<photo> photo { get; set; }
    }
}

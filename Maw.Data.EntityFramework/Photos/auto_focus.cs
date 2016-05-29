using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Photos
{
    [Table("auto_focus", Schema = "photo")]
    public partial class auto_focus
    {
        public auto_focus()
        {
            photo = new HashSet<photo>();
        }

        public short id { get; set; }
        [Required]
        [MaxLength(10)]
        public string name { get; set; }

        [InverseProperty("auto_focus")]
        public virtual ICollection<photo> photo { get; set; }
    }
}

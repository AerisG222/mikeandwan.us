using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Photos
{
    [Table("af_area_mode", Schema = "photo")]
    public partial class af_area_mode
    {
        public af_area_mode()
        {
            photo = new HashSet<photo>();
        }

        public short id { get; set; }
        [Required]
        [MaxLength(40)]
        public string name { get; set; }

        [InverseProperty("af_area_mode")]
        public virtual ICollection<photo> photo { get; set; }
    }
}

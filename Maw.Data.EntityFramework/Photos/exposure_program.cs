using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Photos
{
    [Table("exposure_program", Schema = "photo")]
    public partial class exposure_program
    {
        public exposure_program()
        {
            photo = new HashSet<photo>();
        }

        public int id { get; set; }
        [Required]
        [MaxLength(50)]
        public string name { get; set; }

        [InverseProperty("exposure_program")]
        public virtual ICollection<photo> photo { get; set; }
    }
}

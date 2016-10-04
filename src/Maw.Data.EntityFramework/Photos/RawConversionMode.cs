using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Photos
{
    [Table("raw_conversion_mode", Schema = "photo")]
    public partial class RawConversionMode
    {
        public RawConversionMode()
        {
            Photo = new HashSet<Photo>();
        }

        [Column("id")]
        public short Id { get; set; }
        [Required]
        [Column("name", TypeName = "varchar")]
        [MaxLength(20)]
        public string Name { get; set; }

        [InverseProperty("RawConversionMode")]
        public virtual ICollection<Photo> Photo { get; set; }
    }
}

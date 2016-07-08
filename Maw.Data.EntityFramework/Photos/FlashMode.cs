using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Photos
{
    [Table("flash_mode", Schema = "photo")]
    public partial class FlashMode
    {
        public FlashMode()
        {
            Photo = new HashSet<Photo>();
        }

        [Column("id")]
        public short Id { get; set; }
        [Required]
        [Column("name", TypeName = "varchar")]
        [MaxLength(30)]
        public string Name { get; set; }

        [InverseProperty("FlashMode")]
        public virtual ICollection<Photo> Photo { get; set; }
    }
}

﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Photos
{
    [Table("contrast", Schema = "photo")]
    public partial class Contrast
    {
        public Contrast()
        {
            Photo = new HashSet<Photo>();
        }

        [Column("id")]
        public int Id { get; set; }
        [Required]
        [Column("name", TypeName = "varchar")]
        [MaxLength(10)]
        public string Name { get; set; }

        [InverseProperty("Contrast")]
        public virtual ICollection<Photo> Photo { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Photos
{
    [Table("rating", Schema = "photo")]
    public partial class rating
    {
        public int photo_id { get; set; }
        public short user_id { get; set; }
        public short score { get; set; }

        [ForeignKey("photo_id")]
        [InverseProperty("rating")]
        public virtual photo photo { get; set; }
        [ForeignKey("user_id")]
        [InverseProperty("rating")]
        public virtual user user { get; set; }
    }
}

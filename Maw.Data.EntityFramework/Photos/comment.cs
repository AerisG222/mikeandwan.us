using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Photos
{
    [Table("comment", Schema = "photo")]
    public partial class comment
    {
        public int id { get; set; }
        public DateTime entry_date { get; set; }
        [Required]
        public string message { get; set; }
        public int photo_id { get; set; }
        public short user_id { get; set; }

        [ForeignKey("photo_id")]
        [InverseProperty("comment")]
        public virtual photo photo { get; set; }
        [ForeignKey("user_id")]
        [InverseProperty("comment")]
        public virtual user user { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Photos
{
    [Table("rating", Schema = "photo")]
    public partial class Rating
    {
        [Column("photo_id")]
        public int PhotoId { get; set; }
        [Column("user_id")]
        public short UserId { get; set; }
        [Column("score")]
        public short Score { get; set; }

        [ForeignKey("PhotoId")]
        [InverseProperty("Rating")]
        public virtual Photo Photo { get; set; }
        [ForeignKey("UserId")]
        [InverseProperty("Rating")]
        public virtual User User { get; set; }
    }
}

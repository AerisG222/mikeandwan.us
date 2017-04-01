using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Photos
{
    [Table("comment", Schema = "photo")]
    public partial class Comment
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("photo_id")]
        public int PhotoId { get; set; }
        [Column("user_id")]
        public short UserId { get; set; }
        [Column("entry_date")]
        public DateTime EntryDate { get; set; }
        [Required]
        [Column("message")]
        public string Message { get; set; }

        [ForeignKey("PhotoId")]
        [InverseProperty("Comment")]
        public virtual Photo Photo { get; set; }
        [ForeignKey("UserId")]
        [InverseProperty("Comment")]
        public virtual User User { get; set; }
    }
}

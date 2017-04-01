using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Identity
{
    [Table("login_history", Schema = "maw")]
    public partial class LoginHistory
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("user_id")]
        public short? UserId { get; set; }
        [Required]
        [Column("username", TypeName = "varchar")]
        [MaxLength(30)]
        public string Username { get; set; }
        [Column("attempt_time")]
        public DateTime AttemptTime { get; set; }
        [Column("login_activity_type_id")]
        public short LoginActivityTypeId { get; set; }
        [Column("login_area_id")]
        public short LoginAreaId { get; set; }

        [ForeignKey("LoginActivityTypeId")]
        [InverseProperty("LoginHistory")]
        public virtual LoginActivityType LoginActivityType { get; set; }
        [ForeignKey("LoginAreaId")]
        [InverseProperty("LoginHistory")]
        public virtual LoginArea LoginArea { get; set; }
        [ForeignKey("UserId")]
        [InverseProperty("LoginHistory")]
        public virtual User User { get; set; }
    }
}

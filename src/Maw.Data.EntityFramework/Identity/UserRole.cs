using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Identity
{
    [Table("user_role", Schema = "maw")]
    public partial class UserRole
    {
        [Column("user_id")]
        public short UserId { get; set; }
        [Column("role_id")]
        public short RoleId { get; set; }

        [ForeignKey("RoleId")]
        [InverseProperty("UserRole")]
        public virtual Role Role { get; set; }
        [ForeignKey("UserId")]
        [InverseProperty("UserRole")]
        public virtual User User { get; set; }
    }
}

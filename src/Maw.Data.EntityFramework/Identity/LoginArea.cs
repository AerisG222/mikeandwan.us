using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Identity
{
    [Table("login_area", Schema = "maw")]
    public partial class LoginArea
    {
        public LoginArea()
        {
            LoginHistory = new HashSet<LoginHistory>();
        }

        [Column("id")]
        public short Id { get; set; }
        [Required]
        [Column("name", TypeName = "varchar")]
        [MaxLength(30)]
        public string Name { get; set; }

        [InverseProperty("LoginArea")]
        public virtual ICollection<LoginHistory> LoginHistory { get; set; }
    }
}

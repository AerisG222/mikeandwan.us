using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Identity
{
    [Table("login_activity_type", Schema = "maw")]
    public partial class login_activity_type
    {
        public login_activity_type()
        {
            login_history = new HashSet<login_history>();
        }

        public short id { get; set; }
        [Required]
        [MaxLength(30)]
        public string name { get; set; }

        [InverseProperty("login_activity_type")]
        public virtual ICollection<login_history> login_history { get; set; }
    }
}

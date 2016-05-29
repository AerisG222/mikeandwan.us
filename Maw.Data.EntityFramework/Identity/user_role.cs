using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Identity
{
    [Table("user_role", Schema = "maw")]
    public partial class user_role
    {
        public short user_id { get; set; }
        public short role_id { get; set; }

        [ForeignKey("role_id")]
        [InverseProperty("user_role")]
        public virtual role role { get; set; }
        [ForeignKey("user_id")]
        [InverseProperty("user_role")]
        public virtual user user { get; set; }
    }
}

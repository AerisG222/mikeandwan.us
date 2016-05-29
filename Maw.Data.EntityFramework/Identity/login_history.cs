using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Identity
{
    [Table("login_history", Schema = "maw")]
    public partial class login_history
    {
        public int id { get; set; }
        public DateTime attempt_time { get; set; }
        public short login_activity_type_id { get; set; }
        public short login_area_id { get; set; }
        public short? user_id { get; set; }
        [Required]
        [MaxLength(30)]
        public string username { get; set; }

        [ForeignKey("login_activity_type_id")]
        [InverseProperty("login_history")]
        public virtual login_activity_type login_activity_type { get; set; }
        [ForeignKey("login_area_id")]
        [InverseProperty("login_history")]
        public virtual login_area login_area { get; set; }
        [ForeignKey("user_id")]
        [InverseProperty("login_history")]
        public virtual user user { get; set; }
    }
}

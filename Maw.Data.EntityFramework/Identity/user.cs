using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Identity
{
    [Table("user", Schema = "maw")]
    public partial class user
    {
        public user()
        {
            login_history = new HashSet<login_history>();
            user_role = new HashSet<user_role>();
        }

        public short id { get; set; }
        [MaxLength(100)]
        public string address_1 { get; set; }
        [MaxLength(100)]
        public string address_2 { get; set; }
        [MaxLength(100)]
        public string city { get; set; }
        [MaxLength(50)]
        public string company_name { get; set; }
        public short? country_id { get; set; }
        public DateTime? date_of_birth { get; set; }
        [MaxLength(255)]
        public string email { get; set; }
        [MaxLength(30)]
        public string first_name { get; set; }
        [MaxLength(2000)]
        public string hashed_password { get; set; }
        [MaxLength(50)]
        public string home_phone { get; set; }
        [MaxLength(30)]
        public string last_name { get; set; }
        [MaxLength(50)]
        public string mobile_phone { get; set; }
        public DateTime password_last_set_on { get; set; }
        [MaxLength(50)]
        public string position { get; set; }
        [MaxLength(20)]
        public string postal_code { get; set; }
        [MaxLength(50)]
        public string salt { get; set; }
        [MaxLength(2000)]
        public string security_stamp { get; set; }
        public short? state_id { get; set; }
        [Required]
        [MaxLength(30)]
        public string username { get; set; }
        [MaxLength(255)]
        public string website { get; set; }
        [MaxLength(255)]
        public string work_email { get; set; }
        [MaxLength(50)]
        public string work_phone { get; set; }

        [InverseProperty("user")]
        public virtual ICollection<login_history> login_history { get; set; }
        [InverseProperty("user")]
        public virtual ICollection<user_role> user_role { get; set; }
        [ForeignKey("country_id")]
        [InverseProperty("user")]
        public virtual country country { get; set; }
        [ForeignKey("state_id")]
        [InverseProperty("user")]
        public virtual state state { get; set; }
    }
}

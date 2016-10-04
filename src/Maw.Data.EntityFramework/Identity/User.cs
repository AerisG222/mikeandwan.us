using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maw.Data.EntityFramework.Identity
{
    [Table("user", Schema = "maw")]
    public partial class User
    {
        public User()
        {
            LoginHistory = new HashSet<LoginHistory>();
            UserRole = new HashSet<UserRole>();
        }

        [Column("id")]
        public short Id { get; set; }
        [Required]
        [Column("username", TypeName = "varchar")]
        [MaxLength(30)]
        public string Username { get; set; }
        [Column("salt", TypeName = "varchar")]
        [MaxLength(50)]
        public string Salt { get; set; }
        [Column("hashed_password", TypeName = "varchar")]
        [MaxLength(2000)]
        public string HashedPassword { get; set; }
        [Column("security_stamp", TypeName = "varchar")]
        [MaxLength(2000)]
        public string SecurityStamp { get; set; }
        [Column("password_last_set_on")]
        public DateTime PasswordLastSetOn { get; set; }
        [Column("first_name", TypeName = "varchar")]
        [MaxLength(30)]
        public string FirstName { get; set; }
        [Column("last_name", TypeName = "varchar")]
        [MaxLength(30)]
        public string LastName { get; set; }
        [Required]
        [Column("email", TypeName = "varchar")]
        [MaxLength(255)]
        public string Email { get; set; }
        [Column("website", TypeName = "varchar")]
        [MaxLength(255)]
        public string Website { get; set; }
        [Column("date_of_birth", TypeName = "date")]
        public DateTime? DateOfBirth { get; set; }
        [Column("company_name", TypeName = "varchar")]
        [MaxLength(50)]
        public string CompanyName { get; set; }
        [Column("position", TypeName = "varchar")]
        [MaxLength(50)]
        public string Position { get; set; }
        [Column("work_email", TypeName = "varchar")]
        [MaxLength(255)]
        public string WorkEmail { get; set; }
        [Column("address_1", TypeName = "varchar")]
        [MaxLength(100)]
        public string Address1 { get; set; }
        [Column("address_2", TypeName = "varchar")]
        [MaxLength(100)]
        public string Address2 { get; set; }
        [Column("city", TypeName = "varchar")]
        [MaxLength(100)]
        public string City { get; set; }
        [Column("state_id")]
        public short? StateId { get; set; }
        [Column("postal_code", TypeName = "varchar")]
        [MaxLength(20)]
        public string PostalCode { get; set; }
        [Column("country_id")]
        public short? CountryId { get; set; }
        [Column("home_phone", TypeName = "varchar")]
        [MaxLength(50)]
        public string HomePhone { get; set; }
        [Column("mobile_phone", TypeName = "varchar")]
        [MaxLength(50)]
        public string MobilePhone { get; set; }
        [Column("work_phone", TypeName = "varchar")]
        [MaxLength(50)]
        public string WorkPhone { get; set; }

        [InverseProperty("User")]
        public virtual ICollection<LoginHistory> LoginHistory { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<UserRole> UserRole { get; set; }
        [ForeignKey("CountryId")]
        [InverseProperty("User")]
        public virtual Country Country { get; set; }
        [ForeignKey("StateId")]
        [InverseProperty("User")]
        public virtual State State { get; set; }
    }
}

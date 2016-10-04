using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Maw.Data.EntityFramework.Identity
{
    public partial class IdentityContext : DbContext
    {
        public IdentityContext(DbContextOptions<IdentityContext> opts)
            : base(opts)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Country>(entity =>
            {
                entity.HasIndex(e => e.Code)
                    .HasName("uq_maw_country_code")
                    .IsUnique();

                entity.HasIndex(e => e.Name)
                    .HasName("uq_maw_country_name")
                    .IsUnique();

                entity.Property(e => e.Id).HasDefaultValueSql("nextval('maw.country_id_seq'::regclass)");
            });

            modelBuilder.Entity<LoginActivityType>(entity =>
            {
                entity.HasIndex(e => e.Name)
                    .HasName("uq_maw_login_activity_type_name")
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<LoginArea>(entity =>
            {
                entity.HasIndex(e => e.Name)
                    .HasName("uq_maw_login_area_name")
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<LoginHistory>(entity =>
            {
                entity.HasIndex(e => new { e.UserId, e.AttemptTime })
                    .HasName("ix_maw_login_history_user_id_attempt_time");

                entity.HasIndex(e => new { e.Username, e.AttemptTime })
                    .HasName("ix_maw_login_history_username_attempt_time");

                entity.Property(e => e.Id).HasDefaultValueSql("nextval('maw.login_history_id_seq'::regclass)");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasIndex(e => e.Name)
                    .HasName("uq_maw_role_name")
                    .IsUnique();

                entity.Property(e => e.Id).HasDefaultValueSql("nextval('maw.role_id_seq'::regclass)");
            });

            modelBuilder.Entity<State>(entity =>
            {
                entity.HasIndex(e => e.Code)
                    .HasName("uq_maw_state_code")
                    .IsUnique();

                entity.HasIndex(e => e.Name)
                    .HasName("uq_maw_state_name")
                    .IsUnique();

                entity.Property(e => e.Id).HasDefaultValueSql("nextval('maw.state_id_seq'::regclass)");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Email)
                    .HasName("uq_maw_user_email")
                    .IsUnique();

                entity.HasIndex(e => e.Username)
                    .HasName("uq_maw_user_username")
                    .IsUnique();

                entity.Property(e => e.Id).HasDefaultValueSql("nextval('maw.user_id_seq'::regclass)");
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId })
                    .HasName("PK_user_role");
            });

            modelBuilder.HasSequence("post_id_seq", "blog");

            modelBuilder.HasSequence("login_history_id_seq", "maw");

            modelBuilder.HasSequence("role_id_seq", "maw");

            modelBuilder.HasSequence("user_id_seq", "maw");

            modelBuilder.HasSequence("comment_id_seq", "photo");
        }

        public virtual DbSet<Country> Country { get; set; }
        public virtual DbSet<LoginActivityType> LoginActivityType { get; set; }
        public virtual DbSet<LoginArea> LoginArea { get; set; }
        public virtual DbSet<LoginHistory> LoginHistory { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<State> State { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserRole> UserRole { get; set; }
    }
}
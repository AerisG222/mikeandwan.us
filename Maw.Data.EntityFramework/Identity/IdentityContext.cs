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
            modelBuilder.Entity<country>(entity =>
            {
                entity.HasIndex(e => e.code).HasName("uq_maw_country_code").IsUnique();

                entity.HasIndex(e => e.name).HasName("uq_maw_country_name").IsUnique();

                entity.Property(e => e.id)
                    .HasDefaultValueSql("nextval('maw.country_id_seq'::regclass)")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.code).HasColumnType("bpchar");

                entity.Property(e => e.name).HasColumnType("varchar");
            });

            modelBuilder.Entity<login_activity_type>(entity =>
            {
                entity.HasIndex(e => e.name).HasName("uq_maw_login_activity_type_name").IsUnique();

                entity.Property(e => e.id).ValueGeneratedNever();

                entity.Property(e => e.name).HasColumnType("varchar");
            });

            modelBuilder.Entity<login_area>(entity =>
            {
                entity.HasIndex(e => e.name).HasName("uq_maw_login_area_name").IsUnique();

                entity.Property(e => e.id).ValueGeneratedNever();

                entity.Property(e => e.name).HasColumnType("varchar");
            });

            modelBuilder.Entity<login_history>(entity =>
            {
                entity.HasIndex(e => new { e.user_id, e.attempt_time }).HasName("ix_maw_login_history_user_id_attempt_time");

                entity.HasIndex(e => new { e.username, e.attempt_time }).HasName("ix_maw_login_history_username_attempt_time");

                entity.Property(e => e.id)
                    .HasDefaultValueSql("nextval('maw.login_history_id_seq'::regclass)")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.username).HasColumnType("varchar");
            });

            modelBuilder.Entity<role>(entity =>
            {
                entity.HasIndex(e => e.name).HasName("uq_maw_role_name").IsUnique();

                entity.Property(e => e.id)
                    .HasDefaultValueSql("nextval('maw.role_id_seq'::regclass)")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.description).HasColumnType("varchar");

                entity.Property(e => e.name).HasColumnType("varchar");
            });

            modelBuilder.Entity<state>(entity =>
            {
                entity.HasIndex(e => e.code).HasName("uq_maw_state_code").IsUnique();

                entity.HasIndex(e => e.name).HasName("uq_maw_state_name").IsUnique();

                entity.Property(e => e.id)
                    .HasDefaultValueSql("nextval('maw.state_id_seq'::regclass)")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.code).HasColumnType("bpchar");

                entity.Property(e => e.name).HasColumnType("varchar");
            });

            modelBuilder.Entity<user>(entity =>
            {
                entity.HasIndex(e => e.email).HasName("uq_maw_user_email").IsUnique();

                entity.HasIndex(e => e.username).HasName("uq_maw_user_username").IsUnique();

                entity.Property(e => e.id)
                    .HasDefaultValueSql("nextval('maw.user_id_seq'::regclass)")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.address_1).HasColumnType("varchar");

                entity.Property(e => e.address_2).HasColumnType("varchar");

                entity.Property(e => e.city).HasColumnType("varchar");

                entity.Property(e => e.company_name).HasColumnType("varchar");

                entity.Property(e => e.date_of_birth).HasColumnType("date");

                entity.Property(e => e.email).HasColumnType("varchar");

                entity.Property(e => e.first_name).HasColumnType("varchar");

                entity.Property(e => e.hashed_password).HasColumnType("varchar");

                entity.Property(e => e.home_phone).HasColumnType("varchar");

                entity.Property(e => e.last_name).HasColumnType("varchar");

                entity.Property(e => e.mobile_phone).HasColumnType("varchar");

                entity.Property(e => e.position).HasColumnType("varchar");

                entity.Property(e => e.postal_code).HasColumnType("varchar");

                entity.Property(e => e.salt).HasColumnType("varchar");

                entity.Property(e => e.security_stamp).HasColumnType("varchar");

                entity.Property(e => e.username).HasColumnType("varchar");

                entity.Property(e => e.website).HasColumnType("varchar");

                entity.Property(e => e.work_email).HasColumnType("varchar");

                entity.Property(e => e.work_phone).HasColumnType("varchar");
            });

            modelBuilder.Entity<user_role>(entity =>
            {
                entity.HasKey(e => new { e.user_id, e.role_id });
            });
        }

        public virtual DbSet<country> country { get; set; }
        public virtual DbSet<login_activity_type> login_activity_type { get; set; }
        public virtual DbSet<login_area> login_area { get; set; }
        public virtual DbSet<login_history> login_history { get; set; }
        public virtual DbSet<role> role { get; set; }
        public virtual DbSet<state> state { get; set; }
        public virtual DbSet<user> user { get; set; }
        public virtual DbSet<user_role> user_role { get; set; }
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;


namespace Maw.Data.EntityFramework.Photos
{
    public partial class PhotoContext : DbContext
    {
        public PhotoContext(DbContextOptions<PhotoContext> opts)
            : base(opts)
        {
            
        }
        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<active_d_lighting>(entity =>
            {
                entity.HasIndex(e => e.name).HasName("uq_photo_active_d_lighting_name").IsUnique();

                entity.Property(e => e.id)
                    .HasDefaultValueSql("nextval('photo.active_d_lighting_id_seq'::regclass)")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.name).HasColumnType("varchar");
            });

            modelBuilder.Entity<af_area_mode>(entity =>
            {
                entity.HasIndex(e => e.name).HasName("uq_photo_af_area_mode_name").IsUnique();

                entity.Property(e => e.id)
                    .HasDefaultValueSql("nextval('photo.af_area_mode_id_seq'::regclass)")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.name).HasColumnType("varchar");
            });

            modelBuilder.Entity<af_point>(entity =>
            {
                entity.HasIndex(e => e.name).HasName("uq_photo_af_point_name").IsUnique();

                entity.Property(e => e.id)
                    .HasDefaultValueSql("nextval('photo.af_point_id_seq'::regclass)")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.name).HasColumnType("varchar");
            });

            modelBuilder.Entity<auto_focus>(entity =>
            {
                entity.HasIndex(e => e.name).HasName("uq_photo_auto_focus_name").IsUnique();

                entity.Property(e => e.id).ValueGeneratedNever();

                entity.Property(e => e.name).HasColumnType("varchar");
            });

            modelBuilder.Entity<category>(entity =>
            {
                entity.HasIndex(e => new { e.year, e.is_private }).HasName("ix_photo_category_year_is_private");

                entity.Property(e => e.id)
                    .HasDefaultValueSql("nextval('photo.category_id_seq'::regclass)")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.name).HasColumnType("varchar");

                entity.Property(e => e.teaser_photo_path).HasColumnType("varchar");
            });

            modelBuilder.Entity<colorspace>(entity =>
            {
                entity.HasIndex(e => e.name).HasName("uq_photo_colorspace_name").IsUnique();

                entity.Property(e => e.id)
                    .HasDefaultValueSql("nextval('photo.colorspace_id_seq'::regclass)")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.name).HasColumnType("varchar");
            });

            modelBuilder.Entity<comment>(entity =>
            {
                entity.HasIndex(e => e.photo_id).HasName("ix_photo_comment_photo_id");

                entity.HasIndex(e => e.user_id).HasName("ix_photo_comment_user_id");

                entity.Property(e => e.id)
                    .HasDefaultValueSql("nextval('photo.comment_id_seq'::regclass)")
                    .ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<compression>(entity =>
            {
                entity.Property(e => e.id).ValueGeneratedNever();

                entity.Property(e => e.name).HasColumnType("varchar");
            });

            modelBuilder.Entity<contrast>(entity =>
            {
                entity.HasIndex(e => e.name).HasName("uq_photo_contrast_name").IsUnique();

                entity.Property(e => e.id).ValueGeneratedNever();

                entity.Property(e => e.name).HasColumnType("varchar");
            });

            modelBuilder.Entity<exposure_mode>(entity =>
            {
                entity.HasIndex(e => e.name).HasName("uq_photo_exposure_mode_name").IsUnique();

                entity.Property(e => e.id).ValueGeneratedNever();

                entity.Property(e => e.name).HasColumnType("varchar");
            });

            modelBuilder.Entity<exposure_program>(entity =>
            {
                entity.HasIndex(e => e.name).HasName("uq_photo_exposure_program_name").IsUnique();

                entity.Property(e => e.id).ValueGeneratedNever();

                entity.Property(e => e.name).HasColumnType("varchar");
            });

            modelBuilder.Entity<flash>(entity =>
            {
                entity.HasIndex(e => e.name).HasName("uq_photo_flash_name").IsUnique();

                entity.Property(e => e.id).ValueGeneratedNever();

                entity.Property(e => e.name).HasColumnType("varchar");
            });

            modelBuilder.Entity<flash_color_filter>(entity =>
            {
                entity.HasIndex(e => e.name).HasName("uq_photo_flash_color_filter_name").IsUnique();

                entity.Property(e => e.id)
                    .HasDefaultValueSql("nextval('photo.flash_color_filter_id_seq'::regclass)")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.name).HasColumnType("varchar");
            });

            modelBuilder.Entity<flash_mode>(entity =>
            {
                entity.HasIndex(e => e.name).HasName("uq_photo_flash_mode_name").IsUnique();

                entity.Property(e => e.id)
                    .HasDefaultValueSql("nextval('photo.flash_mode_id_seq'::regclass)")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.name).HasColumnType("varchar");
            });

            modelBuilder.Entity<flash_setting>(entity =>
            {
                entity.HasIndex(e => e.name).HasName("uq_photo_flash_setting_name").IsUnique();

                entity.Property(e => e.id)
                    .HasDefaultValueSql("nextval('photo.flash_setting_id_seq'::regclass)")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.name).HasColumnType("varchar");
            });

            modelBuilder.Entity<flash_type>(entity =>
            {
                entity.HasIndex(e => e.name).HasName("uq_photo_flash_type_name").IsUnique();

                entity.Property(e => e.id)
                    .HasDefaultValueSql("nextval('photo.flash_type_id_seq'::regclass)")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.name).HasColumnType("varchar");
            });

            modelBuilder.Entity<focus_mode>(entity =>
            {
                entity.HasIndex(e => e.name).HasName("uq_photo_focus_mode_name").IsUnique();

                entity.Property(e => e.id)
                    .HasDefaultValueSql("nextval('photo.focus_mode_id_seq'::regclass)")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.name).HasColumnType("varchar");
            });

            modelBuilder.Entity<gain_control>(entity =>
            {
                entity.HasIndex(e => e.name).HasName("uq_photo_gain_control_name").IsUnique();

                entity.Property(e => e.id).ValueGeneratedNever();

                entity.Property(e => e.name).HasColumnType("varchar");
            });

            modelBuilder.Entity<gps_altitude_ref>(entity =>
            {
                entity.HasIndex(e => e.name).HasName("uq_photo_gps_altitude_ref_name").IsUnique();

                entity.Property(e => e.id).ValueGeneratedNever();

                entity.Property(e => e.name).HasColumnType("varchar");
            });

            modelBuilder.Entity<gps_direction_ref>(entity =>
            {
                entity.HasIndex(e => e.name).HasName("uq_photo_gps_direction_ref_name").IsUnique();

                entity.Property(e => e.id).HasColumnType("varchar");

                entity.Property(e => e.name).HasColumnType("varchar");
            });

            modelBuilder.Entity<gps_latitude_ref>(entity =>
            {
                entity.HasIndex(e => e.name).HasName("uq_photo_gps_latitude_ref_name").IsUnique();

                entity.Property(e => e.id).HasColumnType("varchar");

                entity.Property(e => e.name).HasColumnType("varchar");
            });

            modelBuilder.Entity<gps_longitude_ref>(entity =>
            {
                entity.HasIndex(e => e.name).HasName("uq_photo_gps_longitude_ref_name").IsUnique();

                entity.Property(e => e.id).HasColumnType("varchar");

                entity.Property(e => e.name).HasColumnType("varchar");
            });

            modelBuilder.Entity<gps_measure_mode>(entity =>
            {
                entity.HasIndex(e => e.name).HasName("uq_photo_gps_measure_mode_name").IsUnique();

                entity.Property(e => e.id).HasColumnType("varchar");

                entity.Property(e => e.name).HasColumnType("varchar");
            });

            modelBuilder.Entity<gps_status>(entity =>
            {
                entity.HasIndex(e => e.name).HasName("uq_photo_gps_status_name").IsUnique();

                entity.Property(e => e.id).HasColumnType("varchar");

                entity.Property(e => e.name).HasColumnType("varchar");
            });

            modelBuilder.Entity<high_iso_noise_reduction>(entity =>
            {
                entity.HasIndex(e => e.name).HasName("uq_photo_high_iso_noise_reduction_name").IsUnique();

                entity.Property(e => e.id)
                    .HasDefaultValueSql("nextval('photo.high_iso_noise_reduction_id_seq'::regclass)")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.name).HasColumnType("varchar");
            });

            modelBuilder.Entity<hue_adjustment>(entity =>
            {
                entity.HasIndex(e => e.name).HasName("uq_photo_hue_adjustment_name").IsUnique();

                entity.Property(e => e.id)
                    .HasDefaultValueSql("nextval('photo.hue_adjustment_id_seq'::regclass)")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.name).HasColumnType("varchar");
            });

            modelBuilder.Entity<lens>(entity =>
            {
                entity.HasIndex(e => e.name).HasName("uq_photo_lens_name").IsUnique();

                entity.Property(e => e.id)
                    .HasDefaultValueSql("nextval('photo.lens_id_seq'::regclass)")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.name).HasColumnType("varchar");
            });

            modelBuilder.Entity<light_source>(entity =>
            {
                entity.HasIndex(e => e.name).HasName("uq_photo_light_source_name").IsUnique();

                entity.Property(e => e.id).ValueGeneratedNever();

                entity.Property(e => e.name).HasColumnType("varchar");
            });

            modelBuilder.Entity<make>(entity =>
            {
                entity.HasIndex(e => e.name).HasName("uq_photo_make_name").IsUnique();

                entity.Property(e => e.id)
                    .HasDefaultValueSql("nextval('photo.make_id_seq'::regclass)")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.name).HasColumnType("varchar");
            });

            modelBuilder.Entity<metering_mode>(entity =>
            {
                entity.HasIndex(e => e.name).HasName("uq_photo_metering_mode_name").IsUnique();

                entity.Property(e => e.id).ValueGeneratedNever();

                entity.Property(e => e.name).HasColumnType("varchar");
            });

            modelBuilder.Entity<model>(entity =>
            {
                entity.HasIndex(e => e.name).HasName("uq_photo_model_name").IsUnique();

                entity.Property(e => e.id)
                    .HasDefaultValueSql("nextval('photo.model_id_seq'::regclass)")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.name).HasColumnType("varchar");
            });

            modelBuilder.Entity<noise_reduction>(entity =>
            {
                entity.HasIndex(e => e.name).HasName("uq_photo_noise_reduction_name").IsUnique();

                entity.Property(e => e.id)
                    .HasDefaultValueSql("nextval('photo.noise_reduction_id_seq'::regclass)")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.name).HasColumnType("varchar");
            });

            modelBuilder.Entity<orientation>(entity =>
            {
                entity.HasIndex(e => e.name).HasName("uq_photo_orientation_name").IsUnique();

                entity.Property(e => e.id).ValueGeneratedNever();

                entity.Property(e => e.name).HasColumnType("varchar");
            });

            modelBuilder.Entity<photo>(entity =>
            {
                entity.HasIndex(e => new { e.category_id, e.is_private }).HasName("ix_photo_photo_category_id_is_private");

                entity.Property(e => e.id)
                    .HasDefaultValueSql("nextval('photo.photo_id_seq'::regclass)")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.depth_of_field).HasColumnType("varchar");

                entity.Property(e => e.exposure_compensation).HasColumnType("varchar");

                entity.Property(e => e.exposure_time).HasColumnType("varchar");

                entity.Property(e => e.field_of_view).HasColumnType("varchar");

                entity.Property(e => e.flash_compensation).HasColumnType("varchar");

                entity.Property(e => e.flash_exposure_compensation).HasColumnType("varchar");

                entity.Property(e => e.gps_direction_ref_id).HasColumnType("varchar");

                entity.Property(e => e.gps_latitude_ref_id).HasColumnType("varchar");

                entity.Property(e => e.gps_longitude_ref_id).HasColumnType("varchar");

                entity.Property(e => e.gps_measure_mode_id).HasColumnType("varchar");

                entity.Property(e => e.gps_satellites).HasColumnType("varchar");

                entity.Property(e => e.gps_status_id).HasColumnType("varchar");

                entity.Property(e => e.gps_version_id).HasColumnType("varchar");

                entity.Property(e => e.lg_path).HasColumnType("varchar");

                entity.Property(e => e.md_path).HasColumnType("varchar");

                entity.Property(e => e.primary_af_point).HasColumnType("varchar");
                
                entity.Property(e => e.prt_path).HasColumnType("varchar");

                entity.Property(e => e.shutter_speed).HasColumnType("varchar");

                entity.Property(e => e.sm_path).HasColumnType("varchar");

                entity.Property(e => e.src_path).HasColumnType("varchar");

                entity.Property(e => e.xs_path).HasColumnType("varchar");
            });

            modelBuilder.Entity<picture_control_name>(entity =>
            {
                entity.HasIndex(e => e.name).HasName("uq_photo_picture_control_name_name").IsUnique();

                entity.Property(e => e.id)
                    .HasDefaultValueSql("nextval('photo.picture_control_name_id_seq'::regclass)")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.name).HasColumnType("varchar");
            });

            modelBuilder.Entity<rating>(entity =>
            {
                entity.HasKey(e => new { e.photo_id, e.user_id });

                entity.HasIndex(e => e.photo_id).HasName("ix_photo_rating_photo_id");

                entity.HasIndex(e => e.user_id).HasName("ix_photo_rating_user_id");
            });

            modelBuilder.Entity<raw_conversion_mode>(entity =>
            {
                entity.HasIndex(e => e.name).HasName("uq_photo_raw_conversion_mode_name").IsUnique();

                entity.Property(e => e.id).ValueGeneratedNever();

                entity.Property(e => e.name).HasColumnType("varchar");
            });
            
            modelBuilder.Entity<saturation>(entity =>
            {
                entity.HasIndex(e => e.name).HasName("uq_photo_saturation_name").IsUnique();

                entity.Property(e => e.id)
                    .HasDefaultValueSql("nextval('photo.saturation_id_seq'::regclass)")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.name).HasColumnType("varchar");
            });

            modelBuilder.Entity<scene_capture_type>(entity =>
            {
                entity.HasIndex(e => e.name).HasName("uq_photo_scene_capture_type_name").IsUnique();

                entity.Property(e => e.id).ValueGeneratedNever();

                entity.Property(e => e.name).HasColumnType("varchar");
            });

            modelBuilder.Entity<scene_type>(entity =>
            {
                entity.HasIndex(e => e.name).HasName("uq_photo_scene_type_name").IsUnique();

                entity.Property(e => e.id).ValueGeneratedNever();

                entity.Property(e => e.name).HasColumnType("varchar");
            });

            modelBuilder.Entity<sensing_method>(entity =>
            {
                entity.HasIndex(e => e.name).HasName("uq_photo_sensing_method_name").IsUnique();

                entity.Property(e => e.id).ValueGeneratedNever();

                entity.Property(e => e.name).HasColumnType("varchar");
            });

            modelBuilder.Entity<sharpness>(entity =>
            {
                entity.HasIndex(e => e.name).HasName("uq_photo_sharpness_name").IsUnique();

                entity.Property(e => e.id).ValueGeneratedNever();

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

            modelBuilder.Entity<vibration_reduction>(entity =>
            {
                entity.HasIndex(e => e.name).HasName("uq_photo_vibration_reduction_name").IsUnique();

                entity.Property(e => e.id)
                    .HasDefaultValueSql("nextval('photo.vibration_reduction_id_seq'::regclass)")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.name).HasColumnType("varchar");
            });

            modelBuilder.Entity<vignette_control>(entity =>
            {
                entity.HasIndex(e => e.name).HasName("uq_photo_vignette_control_name").IsUnique();

                entity.Property(e => e.id)
                    .HasDefaultValueSql("nextval('photo.vignette_control_id_seq'::regclass)")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.name).HasColumnType("varchar");
            });

            modelBuilder.Entity<vr_mode>(entity =>
            {
                entity.HasIndex(e => e.name).HasName("uq_photo_vr_mode_name").IsUnique();

                entity.Property(e => e.id)
                    .HasDefaultValueSql("nextval('photo.vr_mode_id_seq'::regclass)")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.name).HasColumnType("varchar");
            });

            modelBuilder.Entity<white_balance>(entity =>
            {
                entity.HasIndex(e => e.name).HasName("uq_photo_white_balance_name").IsUnique();

                entity.Property(e => e.id)
                    .HasDefaultValueSql("nextval('photo.white_balance_id_seq'::regclass)")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.name).HasColumnType("varchar");
            });
        }

        public virtual DbSet<active_d_lighting> active_d_lighting { get; set; }
        public virtual DbSet<af_area_mode> af_area_mode { get; set; }
        public virtual DbSet<af_point> af_point { get; set; }
        public virtual DbSet<auto_focus> auto_focus { get; set; }
        public virtual DbSet<category> category { get; set; }
        public virtual DbSet<colorspace> colorspace { get; set; }
        public virtual DbSet<comment> comment { get; set; }
        public virtual DbSet<compression> compression { get; set; }
        public virtual DbSet<contrast> contrast { get; set; }
        public virtual DbSet<exposure_mode> exposure_mode { get; set; }
        public virtual DbSet<exposure_program> exposure_program { get; set; }
        public virtual DbSet<flash> flash { get; set; }
        public virtual DbSet<flash_color_filter> flash_color_filter { get; set; }
        public virtual DbSet<flash_mode> flash_mode { get; set; }
        public virtual DbSet<flash_setting> flash_setting { get; set; }
        public virtual DbSet<flash_type> flash_type { get; set; }
        public virtual DbSet<focus_mode> focus_mode { get; set; }
        public virtual DbSet<gain_control> gain_control { get; set; }
        public virtual DbSet<gps_altitude_ref> gps_altitude_ref { get; set; }
        public virtual DbSet<gps_direction_ref> gps_direction_ref { get; set; }
        public virtual DbSet<gps_latitude_ref> gps_latitude_ref { get; set; }
        public virtual DbSet<gps_longitude_ref> gps_longitude_ref { get; set; }
        public virtual DbSet<gps_measure_mode> gps_measure_mode { get; set; }
        public virtual DbSet<gps_status> gps_status { get; set; }
        public virtual DbSet<high_iso_noise_reduction> high_iso_noise_reduction { get; set; }
        public virtual DbSet<hue_adjustment> hue_adjustment { get; set; }
        public virtual DbSet<lens> lens { get; set; }
        public virtual DbSet<light_source> light_source { get; set; }
        public virtual DbSet<make> make { get; set; }
        public virtual DbSet<metering_mode> metering_mode { get; set; }
        public virtual DbSet<model> model { get; set; }
        public virtual DbSet<noise_reduction> noise_reduction { get; set; }
        public virtual DbSet<orientation> orientation { get; set; }
        public virtual DbSet<photo> photo { get; set; }
        public virtual DbSet<picture_control_name> picture_control_name { get; set; }
        public virtual DbSet<rating> rating { get; set; }
        public virtual DbSet<raw_conversion_mode> raw_conversion_mode { get; set; }
        public virtual DbSet<saturation> saturation { get; set; }
        public virtual DbSet<scene_capture_type> scene_capture_type { get; set; }
        public virtual DbSet<scene_type> scene_type { get; set; }
        public virtual DbSet<sensing_method> sensing_method { get; set; }
        public virtual DbSet<sharpness> sharpness { get; set; }
        public virtual DbSet<user> user { get; set; }
        public virtual DbSet<vibration_reduction> vibration_reduction { get; set; }
        public virtual DbSet<vignette_control> vignette_control { get; set; }
        public virtual DbSet<vr_mode> vr_mode { get; set; }
        public virtual DbSet<white_balance> white_balance { get; set; }
    }
}
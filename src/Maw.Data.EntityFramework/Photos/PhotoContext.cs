using Microsoft.EntityFrameworkCore;

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
            modelBuilder.Entity<ActiveDLighting>(entity =>
            {
                entity.HasIndex(e => e.Name)
                    .HasName("uq_photo_active_d_lighting_name")
                    .IsUnique();

                entity.Property(e => e.Id).HasDefaultValueSql("nextval('photo.active_d_lighting_id_seq'::regclass)");
            });

            modelBuilder.Entity<AfAreaMode>(entity =>
            {
                entity.HasIndex(e => e.Name)
                    .HasName("uq_photo_af_area_mode_name")
                    .IsUnique();

                entity.Property(e => e.Id).HasDefaultValueSql("nextval('photo.af_area_mode_id_seq'::regclass)");
            });

            modelBuilder.Entity<AfPoint>(entity =>
            {
                entity.HasIndex(e => e.Name)
                    .HasName("uq_photo_af_point_name")
                    .IsUnique();

                entity.Property(e => e.Id).HasDefaultValueSql("nextval('photo.af_point_id_seq'::regclass)");
            });

            modelBuilder.Entity<AutoFocus>(entity =>
            {
                entity.HasIndex(e => e.Name)
                    .HasName("uq_photo_auto_focus_name")
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasIndex(e => new { e.Year, e.IsPrivate })
                    .HasName("ix_photo_category_year_is_private");

                entity.Property(e => e.Id).HasDefaultValueSql("nextval('photo.category_id_seq'::regclass)");
            });

            modelBuilder.Entity<Colorspace>(entity =>
            {
                entity.HasIndex(e => e.Name)
                    .HasName("uq_photo_colorspace_name")
                    .IsUnique();

                entity.Property(e => e.Id).HasDefaultValueSql("nextval('photo.colorspace_id_seq'::regclass)");
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.HasIndex(e => e.PhotoId)
                    .HasName("ix_photo_comment_photo_id");

                entity.HasIndex(e => e.UserId)
                    .HasName("ix_photo_comment_user_id");

                entity.Property(e => e.Id).HasDefaultValueSql("nextval('photo.comment_id_seq'::regclass)");
            });

            modelBuilder.Entity<Compression>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<Contrast>(entity =>
            {
                entity.HasIndex(e => e.Name)
                    .HasName("uq_photo_contrast_name")
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<ExposureMode>(entity =>
            {
                entity.HasIndex(e => e.Name)
                    .HasName("uq_photo_exposure_mode_name")
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<ExposureProgram>(entity =>
            {
                entity.HasIndex(e => e.Name)
                    .HasName("uq_photo_exposure_program_name")
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<Flash>(entity =>
            {
                entity.HasIndex(e => e.Name)
                    .HasName("uq_photo_flash_name")
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<FlashColorFilter>(entity =>
            {
                entity.HasIndex(e => e.Name)
                    .HasName("uq_photo_flash_color_filter_name")
                    .IsUnique();

                entity.Property(e => e.Id).HasDefaultValueSql("nextval('photo.flash_color_filter_id_seq'::regclass)");
            });

            modelBuilder.Entity<FlashMode>(entity =>
            {
                entity.HasIndex(e => e.Name)
                    .HasName("uq_photo_flash_mode_name")
                    .IsUnique();

                entity.Property(e => e.Id).HasDefaultValueSql("nextval('photo.flash_mode_id_seq'::regclass)");
            });

            modelBuilder.Entity<FlashSetting>(entity =>
            {
                entity.HasIndex(e => e.Name)
                    .HasName("uq_photo_flash_setting_name")
                    .IsUnique();

                entity.Property(e => e.Id).HasDefaultValueSql("nextval('photo.flash_setting_id_seq'::regclass)");
            });

            modelBuilder.Entity<FlashType>(entity =>
            {
                entity.HasIndex(e => e.Name)
                    .HasName("uq_photo_flash_type_name")
                    .IsUnique();

                entity.Property(e => e.Id).HasDefaultValueSql("nextval('photo.flash_type_id_seq'::regclass)");
            });

            modelBuilder.Entity<FocusMode>(entity =>
            {
                entity.HasIndex(e => e.Name)
                    .HasName("uq_photo_focus_mode_name")
                    .IsUnique();

                entity.Property(e => e.Id).HasDefaultValueSql("nextval('photo.focus_mode_id_seq'::regclass)");
            });

            modelBuilder.Entity<GainControl>(entity =>
            {
                entity.HasIndex(e => e.Name)
                    .HasName("uq_photo_gain_control_name")
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<GpsAltitudeRef>(entity =>
            {
                entity.HasIndex(e => e.Name)
                    .HasName("uq_photo_gps_altitude_ref_name")
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<GpsDirectionRef>(entity =>
            {
                entity.HasIndex(e => e.Name)
                    .HasName("uq_photo_gps_direction_ref_name")
                    .IsUnique();
            });

            modelBuilder.Entity<GpsLatitudeRef>(entity =>
            {
                entity.HasIndex(e => e.Name)
                    .HasName("uq_photo_gps_latitude_ref_name")
                    .IsUnique();
            });

            modelBuilder.Entity<GpsLongitudeRef>(entity =>
            {
                entity.HasIndex(e => e.Name)
                    .HasName("uq_photo_gps_longitude_ref_name")
                    .IsUnique();
            });

            modelBuilder.Entity<GpsMeasureMode>(entity =>
            {
                entity.HasIndex(e => e.Name)
                    .HasName("uq_photo_gps_measure_mode_name")
                    .IsUnique();
            });

            modelBuilder.Entity<GpsStatus>(entity =>
            {
                entity.HasIndex(e => e.Name)
                    .HasName("uq_photo_gps_status_name")
                    .IsUnique();
            });

            modelBuilder.Entity<HighIsoNoiseReduction>(entity =>
            {
                entity.HasIndex(e => e.Name)
                    .HasName("uq_photo_high_iso_noise_reduction_name")
                    .IsUnique();

                entity.Property(e => e.Id).HasDefaultValueSql("nextval('photo.high_iso_noise_reduction_id_seq'::regclass)");
            });

            modelBuilder.Entity<HueAdjustment>(entity =>
            {
                entity.HasIndex(e => e.Name)
                    .HasName("uq_photo_hue_adjustment_name")
                    .IsUnique();

                entity.Property(e => e.Id).HasDefaultValueSql("nextval('photo.hue_adjustment_id_seq'::regclass)");
            });

            modelBuilder.Entity<Lens>(entity =>
            {
                entity.HasIndex(e => e.Name)
                    .HasName("uq_photo_lens_name")
                    .IsUnique();

                entity.Property(e => e.Id).HasDefaultValueSql("nextval('photo.lens_id_seq'::regclass)");
            });

            modelBuilder.Entity<LightSource>(entity =>
            {
                entity.HasIndex(e => e.Name)
                    .HasName("uq_photo_light_source_name")
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<Make>(entity =>
            {
                entity.HasIndex(e => e.Name)
                    .HasName("uq_photo_make_name")
                    .IsUnique();

                entity.Property(e => e.Id).HasDefaultValueSql("nextval('photo.make_id_seq'::regclass)");
            });

            modelBuilder.Entity<MeteringMode>(entity =>
            {
                entity.HasIndex(e => e.Name)
                    .HasName("uq_photo_metering_mode_name")
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<Model>(entity =>
            {
                entity.HasIndex(e => e.Name)
                    .HasName("uq_photo_model_name")
                    .IsUnique();

                entity.Property(e => e.Id).HasDefaultValueSql("nextval('photo.model_id_seq'::regclass)");
            });

            modelBuilder.Entity<NoiseReduction>(entity =>
            {
                entity.HasIndex(e => e.Name)
                    .HasName("uq_photo_noise_reduction_name")
                    .IsUnique();

                entity.Property(e => e.Id).HasDefaultValueSql("nextval('photo.noise_reduction_id_seq'::regclass)");
            });

            modelBuilder.Entity<Orientation>(entity =>
            {
                entity.HasIndex(e => e.Name)
                    .HasName("uq_photo_orientation_name")
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<Photo>(entity =>
            {
                entity.HasIndex(e => new { e.CategoryId, e.IsPrivate })
                    .HasName("ix_photo_photo_category_id_is_private");

                entity.Property(e => e.Id).HasDefaultValueSql("nextval('photo.photo_id_seq'::regclass)");
            });

            modelBuilder.Entity<PictureControlName>(entity =>
            {
                entity.HasIndex(e => e.Name)
                    .HasName("uq_photo_picture_control_name_name")
                    .IsUnique();

                entity.Property(e => e.Id).HasDefaultValueSql("nextval('photo.picture_control_name_id_seq'::regclass)");
            });

            modelBuilder.Entity<Rating>(entity =>
            {
                entity.HasKey(e => new { e.PhotoId, e.UserId })
                    .HasName("PK_rating");

                entity.HasIndex(e => e.PhotoId)
                    .HasName("ix_photo_rating_photo_id");

                entity.HasIndex(e => e.UserId)
                    .HasName("ix_photo_rating_user_id");
            });

            modelBuilder.Entity<RawConversionMode>(entity =>
            {
                entity.HasIndex(e => e.Name)
                    .HasName("uq_photo_raw_conversion_mode_name")
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<Saturation>(entity =>
            {
                entity.HasIndex(e => e.Name)
                    .HasName("uq_photo_saturation_name")
                    .IsUnique();

                entity.Property(e => e.Id).HasDefaultValueSql("nextval('photo.saturation_id_seq'::regclass)");
            });

            modelBuilder.Entity<SceneCaptureType>(entity =>
            {
                entity.HasIndex(e => e.Name)
                    .HasName("uq_photo_scene_capture_type_name")
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<SceneType>(entity =>
            {
                entity.HasIndex(e => e.Name)
                    .HasName("uq_photo_scene_type_name")
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<SensingMethod>(entity =>
            {
                entity.HasIndex(e => e.Name)
                    .HasName("uq_photo_sensing_method_name")
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<Sharpness>(entity =>
            {
                entity.HasIndex(e => e.Name)
                    .HasName("uq_photo_sharpness_name")
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();
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

            modelBuilder.Entity<VibrationReduction>(entity =>
            {
                entity.HasIndex(e => e.Name)
                    .HasName("uq_photo_vibration_reduction_name")
                    .IsUnique();

                entity.Property(e => e.Id).HasDefaultValueSql("nextval('photo.vibration_reduction_id_seq'::regclass)");
            });

            modelBuilder.Entity<VignetteControl>(entity =>
            {
                entity.HasIndex(e => e.Name)
                    .HasName("uq_photo_vignette_control_name")
                    .IsUnique();

                entity.Property(e => e.Id).HasDefaultValueSql("nextval('photo.vignette_control_id_seq'::regclass)");
            });

            modelBuilder.Entity<VrMode>(entity =>
            {
                entity.HasIndex(e => e.Name)
                    .HasName("uq_photo_vr_mode_name")
                    .IsUnique();

                entity.Property(e => e.Id).HasDefaultValueSql("nextval('photo.vr_mode_id_seq'::regclass)");
            });

            modelBuilder.Entity<WhiteBalance>(entity =>
            {
                entity.HasIndex(e => e.Name)
                    .HasName("uq_photo_white_balance_name")
                    .IsUnique();

                entity.Property(e => e.Id).HasDefaultValueSql("nextval('photo.white_balance_id_seq'::regclass)");
            });

            modelBuilder.HasSequence("post_id_seq", "blog");

            modelBuilder.HasSequence("login_history_id_seq", "maw");

            modelBuilder.HasSequence("role_id_seq", "maw");

            modelBuilder.HasSequence("user_id_seq", "maw");

            modelBuilder.HasSequence("comment_id_seq", "photo");
        }

        public virtual DbSet<ActiveDLighting> ActiveDLighting { get; set; }
        public virtual DbSet<AfAreaMode> AfAreaMode { get; set; }
        public virtual DbSet<AfPoint> AfPoint { get; set; }
        public virtual DbSet<AutoFocus> AutoFocus { get; set; }
        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<Colorspace> Colorspace { get; set; }
        public virtual DbSet<Comment> Comment { get; set; }
        public virtual DbSet<Compression> Compression { get; set; }
        public virtual DbSet<Contrast> Contrast { get; set; }
        public virtual DbSet<ExposureMode> ExposureMode { get; set; }
        public virtual DbSet<ExposureProgram> ExposureProgram { get; set; }
        public virtual DbSet<Flash> Flash { get; set; }
        public virtual DbSet<FlashColorFilter> FlashColorFilter { get; set; }
        public virtual DbSet<FlashMode> FlashMode { get; set; }
        public virtual DbSet<FlashSetting> FlashSetting { get; set; }
        public virtual DbSet<FlashType> FlashType { get; set; }
        public virtual DbSet<FocusMode> FocusMode { get; set; }
        public virtual DbSet<GainControl> GainControl { get; set; }
        public virtual DbSet<GpsAltitudeRef> GpsAltitudeRef { get; set; }
        public virtual DbSet<GpsDirectionRef> GpsDirectionRef { get; set; }
        public virtual DbSet<GpsLatitudeRef> GpsLatitudeRef { get; set; }
        public virtual DbSet<GpsLongitudeRef> GpsLongitudeRef { get; set; }
        public virtual DbSet<GpsMeasureMode> GpsMeasureMode { get; set; }
        public virtual DbSet<GpsStatus> GpsStatus { get; set; }
        public virtual DbSet<HighIsoNoiseReduction> HighIsoNoiseReduction { get; set; }
        public virtual DbSet<HueAdjustment> HueAdjustment { get; set; }
        public virtual DbSet<Lens> Lens { get; set; }
        public virtual DbSet<LightSource> LightSource { get; set; }
        public virtual DbSet<Make> Make { get; set; }
        public virtual DbSet<MeteringMode> MeteringMode { get; set; }
        public new DbSet<Model> Model { get; set; }
        public virtual DbSet<NoiseReduction> NoiseReduction { get; set; }
        public virtual DbSet<Orientation> Orientation { get; set; }
        public virtual DbSet<Photo> Photo { get; set; }
        public virtual DbSet<PictureControlName> PictureControlName { get; set; }
        public virtual DbSet<Rating> Rating { get; set; }
        public virtual DbSet<RawConversionMode> RawConversionMode { get; set; }
        public virtual DbSet<Saturation> Saturation { get; set; }
        public virtual DbSet<SceneCaptureType> SceneCaptureType { get; set; }
        public virtual DbSet<SceneType> SceneType { get; set; }
        public virtual DbSet<SensingMethod> SensingMethod { get; set; }
        public virtual DbSet<Sharpness> Sharpness { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<VibrationReduction> VibrationReduction { get; set; }
        public virtual DbSet<VignetteControl> VignetteControl { get; set; }
        public virtual DbSet<VrMode> VrMode { get; set; }
        public virtual DbSet<WhiteBalance> WhiteBalance { get; set; }
    }
}
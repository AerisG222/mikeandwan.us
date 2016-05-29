using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;


namespace Maw.Data.EntityFramework.Videos
{
    public partial class VideoContext : DbContext
    {
        public VideoContext(DbContextOptions<VideoContext> opts)
            : base(opts)
        {
            
        }
        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<category>(entity =>
            {
                entity.HasIndex(e => new { e.year, e.is_private }).HasName("ix_video_category_year_is_private");

                entity.Property(e => e.id)
                    .HasDefaultValueSql("nextval('video.category_id_seq'::regclass)")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.name).HasColumnType("varchar");

                entity.Property(e => e.teaser_image_path).HasColumnType("varchar");
            });

            modelBuilder.Entity<video>(entity =>
            {
                entity.HasIndex(e => new { e.category_id, e.is_private }).HasName("ix_video_video_category_id_is_private");

                entity.Property(e => e.id)
                    .HasDefaultValueSql("nextval('video.video_id_seq'::regclass)")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.full_path).HasColumnType("varchar");

                entity.Property(e => e.raw_path).HasColumnType("varchar");

                entity.Property(e => e.scaled_path).HasColumnType("varchar");

                entity.Property(e => e.thumb_path).HasColumnType("varchar");
            });
        }

        public virtual DbSet<category> category { get; set; }
        public virtual DbSet<video> video { get; set; }
    }
}
using Microsoft.EntityFrameworkCore;

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
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasIndex(e => new { e.Year, e.IsPrivate })
                    .HasName("ix_video_category_year_is_private");

                entity.Property(e => e.Id).HasDefaultValueSql("nextval('video.category_id_seq'::regclass)");
            });

            modelBuilder.Entity<Video>(entity =>
            {
                entity.HasIndex(e => new { e.CategoryId, e.IsPrivate })
                    .HasName("ix_video_video_category_id_is_private");

                entity.Property(e => e.Id).HasDefaultValueSql("nextval('video.video_id_seq'::regclass)");
            });

            modelBuilder.HasSequence("post_id_seq", "blog");

            modelBuilder.HasSequence("login_history_id_seq", "maw");

            modelBuilder.HasSequence("role_id_seq", "maw");

            modelBuilder.HasSequence("user_id_seq", "maw");

            modelBuilder.HasSequence("comment_id_seq", "photo");
        }

        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<Video> Video { get; set; }
    }
}
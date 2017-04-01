using Microsoft.EntityFrameworkCore;

namespace Maw.Data.EntityFramework.Blogs
{
    public partial class BlogContext : DbContext
    {
        public BlogContext(DbContextOptions<BlogContext> opts)
            : base(opts)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Blog>(entity =>
            {
                entity.HasIndex(e => e.Title)
                    .HasName("uq_blog_blog$title")
                    .IsUnique();

                entity.Property(e => e.Id).HasDefaultValueSql("nextval('blog.blog_id_seq'::regclass)");
            });

            modelBuilder.Entity<Post>(entity =>
            {
                entity.HasIndex(e => new { e.BlogId, e.PublishDate })
                    .HasName("ix_blog_post_blog_id_publish_date");

                entity.Property(e => e.Id).HasDefaultValueSql("nextval('blog.post_id_seq'::regclass)");
            });

            modelBuilder.HasSequence("post_id_seq", "blog");

            modelBuilder.HasSequence("login_history_id_seq", "maw");

            modelBuilder.HasSequence("role_id_seq", "maw");

            modelBuilder.HasSequence("user_id_seq", "maw");

            modelBuilder.HasSequence("comment_id_seq", "photo");
        }

        public virtual DbSet<Blog> Blog { get; set; }
        public virtual DbSet<Post> Post { get; set; }
    }
}
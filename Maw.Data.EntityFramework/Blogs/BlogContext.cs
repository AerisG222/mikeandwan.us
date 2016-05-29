using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;


namespace Maw.Data.EntityFramework.Blogs
{
    public partial class BlogContext 
        : DbContext
    {
        public BlogContext(DbContextOptions<BlogContext> opts)
            : base(opts)
        {
            
        }
        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<blog>(entity =>
            {
                entity.HasIndex(e => e.title).HasName("uq_blog_blog$title").IsUnique();

                entity.Property(e => e.id)
                    .HasDefaultValueSql("nextval('blog.blog_id_seq'::regclass)")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.copyright).HasColumnType("varchar");

                entity.Property(e => e.description).HasColumnType("varchar");

                entity.Property(e => e.title).HasColumnType("varchar");
            });

            modelBuilder.Entity<post>(entity =>
            {
                entity.HasIndex(e => new { e.blog_id, e.publish_date }).HasName("ix_blog_post_blog_id_publish_date");

                entity.Property(e => e.id)
                    .HasDefaultValueSql("nextval('blog.post_id_seq'::regclass)")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.title).HasColumnType("varchar");
            });
        }

        public virtual DbSet<blog> blog { get; set; }
        public virtual DbSet<post> post { get; set; }
    }
}
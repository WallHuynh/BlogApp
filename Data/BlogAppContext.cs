using BlogApp.Areas.Identity.Data;
using BlogApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogApp.Data;

public class BlogAppContext : IdentityDbContext<BlogAppUser>
{
    public BlogAppContext(DbContextOptions<BlogAppContext> options)
        : base(options) { }

    public DbSet<Category> Categories { get; set; }
    public DbSet<Contact> Contacts { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public object AspNetUsers { get; internal set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

            builder
                .Entity<Post>()
                .HasOne(p => p.BlogAppUser)
                .WithMany(u => u.Posts)
                .HasForeignKey(p => p.BlogUserID);
            builder
                .Entity<Post>()
                .HasOne(p => p.Category)
                .WithMany(u => u.Posts)
                .HasForeignKey(p => p.CategoryID);
            builder
                .Entity<Comment>()
                .HasOne(p => p.Post)
                .WithMany(u => u.Comments)
                .HasForeignKey(p => p.PostID);

            builder.Ignore<List<string>>();
            // Other configurations...
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
            builder.ApplyConfiguration(new ApplicationUserIdentityConfiguration());
    }
}

public class ApplicationUserIdentityConfiguration : IEntityTypeConfiguration<BlogAppUser>
{
    public void Configure(EntityTypeBuilder<BlogAppUser> builder)
    {
        builder.Property(u => u.FirstName).HasMaxLength(255);
        builder.Property(u => u.LastName).HasMaxLength(255);
    }
}

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SeeSay.Models.Entities;

namespace SeeSay.Models.Contexts;

public class SqlServerDbContext : IdentityDbContext<User>
{
    public DbSet<Category>? Categories { get; set; }
    public DbSet<Comment>? Comments { get; set; }
    public DbSet<Like>? Likes { get; set; }
    public DbSet<Post>? Posts { get; set; }
    public DbSet<SocialMediaLink>? SocialMediaLinks { get; set; }

    public SqlServerDbContext(DbContextOptions options) : base(options)
    {
        //empty
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(builder =>
        {
            builder.HasKey(category => category.Id);

            builder.HasMany(category => category.Posts)
                .WithMany(post => post.Categories);

            builder.Property(category => category.Name)
                .HasMaxLength(255)
                .IsUnicode(false)
                .IsRequired();
        });
        modelBuilder.Entity<Category>()
            .ToTable(nameof(Categories),
                builder => builder.HasCheckConstraint($"CK_{nameof(Categories)}_{nameof(Category.Name)}",
                    $"[{nameof(Category.Name)}] != ''"));

        modelBuilder.Entity<Comment>(builder =>
        {
            builder.HasKey(comment => comment.Id);

            builder.HasOne(comment => comment.User)
                .WithMany(user => user.Comments)
                .HasForeignKey(comment => comment.UserId);

            builder.HasOne(comment => comment.Post)
                .WithMany(post => post.Comments)
                .HasForeignKey(comment => comment.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(comment => comment.Text)
                .HasColumnType("nvarchar(2048)")
                .HasMaxLength(2048)
                .IsUnicode()
                .IsRequired();

            builder.Property(comment => comment.CreatedAt)
                .HasDefaultValueSql("GETDATE()");
        });
        modelBuilder.Entity<Comment>()
            .ToTable(nameof(Comments),
                builder => builder.HasCheckConstraint($"CK_{nameof(Comments)}_{nameof(Comment.Text)}",
                    $"[{nameof(Comment.Text)}] != ''"));

        modelBuilder.Entity<Like>(builder =>
        {
            builder.HasKey(like => like.Id);

            builder.HasOne(like => like.User)
                .WithMany(user => user.Likes)
                .HasForeignKey(like => like.UserId);

            builder.HasOne(like => like.Post)
                .WithMany(post => post.Likes)
                .HasForeignKey(like => like.PostId);
        });
        modelBuilder.Entity<Like>()
            .ToTable(nameof(Likes));

        modelBuilder.Entity<Post>(builder =>
        {
            builder.HasKey(post => post.Id);

            builder.HasMany(post => post.Categories)
                .WithMany(category => category.Posts);

            builder.HasOne(post => post.User)
                .WithMany(user => user.Posts)
                .HasForeignKey(post => post.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(post => post.Description)
                .HasColumnType("nvarchar(255)")
                .HasMaxLength(255)
                .IsUnicode()
                .IsRequired(false);

            builder.Property(post => post.ImagePath)
                .HasColumnType("varchar(255)")
                .HasMaxLength(255)
                .IsUnicode(false)
                .IsRequired(false);

            builder.Property(post => post.NumberOfDownloads)
                .HasDefaultValue(0)
                .IsRequired();

            builder.Property(post => post.NumberOfViews)
                .HasDefaultValue(0)
                .IsRequired();

            builder.Property(post => post.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            builder.Property(post => post.IsPremium)
                .HasDefaultValue(false);
        });
        modelBuilder.Entity<Post>()
            .ToTable(nameof(Posts));

        modelBuilder.Entity<SocialMediaLink>(builder =>
        {
            builder.HasKey(link => link.Id);

            builder.HasOne(link => link.User)
                .WithMany(user => user.SocialMediaLinks)
                .HasForeignKey(link => link.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(link => link.Link)
                .HasColumnType("varchar(255)")
                .HasMaxLength(255)
                .IsUnicode(false)
                .IsRequired();
        });
        modelBuilder.Entity<SocialMediaLink>()
            .ToTable(nameof(SocialMediaLinks),
                builder => builder.HasCheckConstraint(
                    $"CK_{nameof(SocialMediaLinks)}_{nameof(SocialMediaLink.Link)}",
                    $"[{nameof(SocialMediaLink.Link)}] != ''"));

        modelBuilder.Entity<User>(builder =>
        {
            builder.HasMany(user => user.SocialMediaLinks)
                .WithOne(link => link.User)
                .HasForeignKey(link => link.UserId);

            builder.HasMany(user => user.Posts)
                .WithOne(post => post.User)
                .HasForeignKey(post => post.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(user => user.Likes)
                .WithOne(like => like.User)
                .HasForeignKey(like => like.UserId);

            builder.Property(user => user.HasPremium)
                .HasDefaultValue(false);
            
            builder.Property(user => user.AvatarImagePath)
                .HasDefaultValue("https://localhost:7042/user.png");
        });


        base.OnModelCreating(modelBuilder);
    }
}
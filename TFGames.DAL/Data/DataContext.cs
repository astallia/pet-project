using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TFGames.DAL.Entities;

namespace TFGames.DAL.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        public DbSet<Avatar> Avatars { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Article> Article { get; set; }

        public DbSet<ArticleImage> ArticleImages { get; set; }

        public DbSet<ArticleContent> ArticleContents { get; set; }

        public DbSet<ApplicationSettings> ApplicationSettings { get; set; }

        public DbSet<GameInfo> GameInfo { get; set; }

        public DbSet<Tag> Tags { get; set; }

        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Article>()
                .HasKey(x => x.Id);

            modelBuilder
                .Entity<Article>()
                .Property(x => x.Name)
                .HasMaxLength(100);

            modelBuilder
                .Entity<Article>()
                .Property(x => x.Description)
                .HasMaxLength(1000);

            modelBuilder
                .Entity<GameInfo>()
                .HasKey(x => x.Id);

            modelBuilder
                .Entity<GameInfo>()
                .Property(x => x.GameType)
                .HasMaxLength(100);

            modelBuilder
                .Entity<GameInfo>()
                .Property(x => x.Platform)
                .HasMaxLength(100);

            modelBuilder
                .Entity<GameInfo>()
                .Property(x => x.Year)
                .HasMaxLength(4);

            modelBuilder
                .Entity<Tag>()
                .HasKey(x => x.Id);
            
            modelBuilder
                .Entity<Tag>()
                .HasIndex(x => x.Name)
                .IsUnique();

            modelBuilder.Entity<Comment>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<Article>()
                .HasOne(a => a.Author);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Favorites)
                .WithMany(a => a.Likes);

            base.OnModelCreating(modelBuilder);
        }
    }
}

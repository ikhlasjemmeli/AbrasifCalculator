using Calculator.Models;
using Microsoft.EntityFrameworkCore;

namespace Calculator.Services
{
    public class ApplicationDbContext :DbContext
    {
        public ApplicationDbContext(DbContextOptions options ) :base(options) 
        {
            
        }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Component> Components { get; set; }
        public DbSet<ArticleComponent> ArticleComponents { get; set; }
        public DbSet<User> Users { get; set; }

   
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ArticleComponent>()
                .HasKey(ac => new { ac.ArticleId, ac.ComponentId });

            modelBuilder.Entity<ArticleComponent>()
                .HasOne(ac => ac.Article)
                .WithMany(a => a.ArticleComponents)
                .HasForeignKey(ac => ac.ArticleId);

            modelBuilder.Entity<ArticleComponent>()
                .HasOne(ac => ac.Component)
                .WithMany(c => c.ArticleComponents)
                .HasForeignKey(ac => ac.ComponentId);
        }

    }
}

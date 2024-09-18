using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class APIContext : DbContext
    {
        public APIContext(DbContextOptions<APIContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql("server=localhost;port=3306;database=bancoteste1;user=root;password=47361903;",
                new MySqlServerVersion(new Version(8, 0, 39)));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Movie>()
                .HasMany(m => m.Reviews)
                .WithMany();

            modelBuilder.Entity<User>()
               .HasMany(u => u.Friends)  // Um User tem muitos Friends
               .WithMany();

            // Configurando a relação de um-para-muitos entre User e Review
            modelBuilder.Entity<User>()
                .HasMany(u => u.Reviews)  // Um User tem muitas Reviews
                .WithOne(r => r.User)      // Cada Review pertence a um User
                .HasForeignKey(r => r.UserId);  // A chave estrangeira em Review é UserId

        }

        public DbSet<Movie> Movie { get; set; } = default!;
        public DbSet<Review> Review { get; set; } = default!;
        public DbSet<User> User { get; set; } = default!;
    }
}
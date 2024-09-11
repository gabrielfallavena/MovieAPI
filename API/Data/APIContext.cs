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

        public DbSet<Movie> Movie { get; set; } = default!;
        public DbSet<Review> Reviews { get; set; } = default!;
    }
}
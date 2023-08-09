using Blog.Data.Mappings;
using Blog.Models;
using Microsoft.EntityFrameworkCore;

namespace Blog.Data
{
    public class BlogDataContext : DbContext
    {
        public BlogDataContext(DbContextOptions<BlogDataContext> options)
            : base(options) // estou passando options para a classe pai/base
        {
            
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Post> Posts { get; set; }

        public DbSet<User> Users { get; set; }
        // public DbSet<UserRole> UserRoles { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder options)
        //    => options.UseSqlServer("Server=.\\SQLEXPRESS;Database=Blog;Integrated Security=SSPI;TrustServerCertificate=True");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CategoryMap());
            modelBuilder.ApplyConfiguration(new UserMap());
            modelBuilder.ApplyConfiguration(new PostMap());
        }
    }
}
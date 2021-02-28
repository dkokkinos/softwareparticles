using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManyToManyDotNet5
{
    public class DirectManyToManyRelWithoutJoinTable
    {
        public class UserContext : DbContext
        {
            public DbSet<User> Users { get; set; }
            public DbSet<Book> Books { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseInMemoryDatabase("users-db");
            }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {

            }
        }


        public class User
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public ICollection<Book> BooksRead { get; set; }
        }

        public class Book
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public ICollection<User> Users { get; set; }
        }

        public async Task Run()
        {
            await using var ctx = new UserContext();

            User jim = new User() { Name = "Jim" };
            User nick = new User() { Name = "Nick" };

            Book designPatterns = new Book() { Name = "Design Patterns", Users = new List<User> { jim, nick } };
            Book refactoring = new Book() { Name = "Refactoring", Users = new List<User> { jim } };

            ctx.AddRange(jim, nick, designPatterns, refactoring);
            await ctx.SaveChangesAsync();

            var users = await ctx.Users.Where(u => u.BooksRead.Any(b => b.Name == "Design Patterns")).ToListAsync();
            foreach (var user in users)
            {
                Console.WriteLine("User: " + user.Name);
            }


        }
    }
}

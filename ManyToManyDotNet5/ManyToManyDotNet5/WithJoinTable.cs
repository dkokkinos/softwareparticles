using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManyToManyDotNet5
{
    public class WithJoinTable
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
            public ICollection<UserBook> BooksRead { get; set; }
        }

        public class Book
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public ICollection<UserBook> UserBooks { get; set; }
        }

        public class UserBook
        {
            public int Id { get; set; }
            public User User { get; set; }
            public Book Book { get; set; }
        }

        public async Task Run()
        {
            await using var ctx = new UserContext();

            User jim = new User() { Name = "Jim" };
            User nick = new User() { Name = "Nick" };

            Book designPatterns = new Book() { Name = "Design Patterns" };
            Book refactoring = new Book() { Name = "refactoring" };

            UserBook userBook1 = new UserBook() { User = jim, Book = designPatterns };
            UserBook userBook2 = new UserBook() { User = jim, Book = refactoring };
            UserBook userBook3 = new UserBook() { User = nick, Book = refactoring };

            ctx.AddRange(jim, nick, designPatterns, refactoring, userBook1, userBook2, userBook3);
            await ctx.SaveChangesAsync();

            var users = await ctx.Users.Where(u => u.BooksRead.Any(us => us.Book.Name == "Design Patterns")).ToListAsync();
            foreach (var user in users)
            {
                Console.WriteLine("User: " + user.Name);
            }


        }
    }
}

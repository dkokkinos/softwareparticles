using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManyToManyDotNet5
{
    public class DirectManyToManyRelWithJoinTable
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
                modelBuilder.Entity<User>()
                    .HasMany(u => u.BooksRead)
                    .WithMany(b => b.Users)
                    .UsingEntity<UserBook>(
                        j => j.HasOne(ub => ub.Book).WithMany(u => u.Readers),
                        j => j.HasOne(ub => ub.User).WithMany(b => b.UserBooksDetails));
            }
        }

        public class User
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public ICollection<Book> BooksRead { get; set; }

            public ICollection<UserBook> UserBooksDetails { get; set; }
        }

        public class Book
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public ICollection<User> Users { get; set; }

            public ICollection<UserBook> Readers { get; set; }
        }

        public class UserBook
        {
            public int Id { get;set; }
            public User User { get; set; }
            public Book Book { get; set; }
            public DateTime ReadOn { get; set; }
        }

        public async Task Run()
        {
            await using var ctx = new UserContext();

            User jim = new User() { Name = "Jim" };
            User nick = new User() { Name = "Nick" };

            Book designPatterns = new Book() { Name = "Design Patterns" };
            Book refactoring = new Book() { Name = "refactoring" };

            UserBook userBook1 = new UserBook() { User = jim, Book = designPatterns, ReadOn = new DateTime(2020, 1,1) };
            UserBook userBook2 = new UserBook() { User = jim, Book = refactoring, ReadOn = new DateTime(2010, 1, 1) };
            UserBook userBook3 = new UserBook() { User = nick, Book = refactoring, ReadOn = new DateTime(2020, 2, 1) };

            ctx.AddRange(jim, nick, designPatterns, refactoring, userBook1, userBook2, userBook3);

            await ctx.SaveChangesAsync();

            var users = await ctx.Users.Where(u => u.BooksRead.Any(b => b.Name == "Design Patterns")).ToListAsync();
            foreach (var user in users)
            {
                Console.WriteLine("User: " + user.Name);
            }

            var oldReaders = await ctx.Users.Where(u => u.UserBooksDetails.Any(d => d.ReadOn < new DateTime(2010, 5, 5))).ToListAsync();
            foreach (var user in oldReaders)
            {
                Console.WriteLine("OldReader: " + user.Name);
            }
        }
    }
}

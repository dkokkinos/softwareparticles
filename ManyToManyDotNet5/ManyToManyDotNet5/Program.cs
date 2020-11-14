using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManyToManyDotNet5
{
    class Program
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

        static async Task Main(string[] args)
        {
            DirectManyToManyRelWithoutJoinTable t = new DirectManyToManyRelWithoutJoinTable();
            await t.Run();
        }
    }
}

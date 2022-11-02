using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Bonapio4Database
{
    public class StudentContext : DbContext
    {
        public DbSet<Server> Servers { get; set; }
        public DbSet<Club> Clubs { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Club>().HasKey(c => new { c.Name, c.ServerId });
            modelBuilder.Entity<Profile>().HasKey(c => new { c.UserId, c.ServerId });
            modelBuilder.Entity<Student>().HasKey(c => new { c.UserId, c.ServerId });
        }


        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseMySql("server=localhost;user=root;database=test;port=3306;Connect Timeout=5;");
    }

    public class Server
    {
        [Key]
        public ulong Id { get; set; }
        public string Prefix { get; set; }
        public ulong Logs { get; set; }
        public ulong Spam { get; set; }
    }

    public class Club
    {
        [Key]
        public string Name { get; set; }
        [Key]
        public ulong ServerId { get; set; }
        public ulong RoleId { get; set; }
    }

    public class Profile
    {
        [Key]
        public ulong UserId { get; set; }
        [Key]
        public ulong ServerId { get; set; }
        public int Xp { get; set; }
        public int Level => Xp / 100;
    }
    public class Student
    {
        [Key]
        public ulong UserId { get; set; }
        [Key]
        public ulong ServerId { get; set; }
        public string Grade { get; set; }
        public List<Club> Club { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
    }
}

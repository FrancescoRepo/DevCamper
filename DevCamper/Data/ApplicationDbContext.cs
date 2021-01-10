using DevCamper.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevCamper.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Bootcamp>().HasIndex(b => b.Name).IsUnique();
            builder.Entity<Bootcamp>().HasIndex(b => b.Description).IsUnique();
            builder.Entity<Bootcamp>().HasIndex(b => b.Website).IsUnique();
            builder.Entity<Bootcamp>().HasIndex(b => b.Phone).IsUnique();

            builder.Entity<Course>().HasIndex(c => c.Title).IsUnique();
            builder.Entity<Course>().HasIndex(c => c.Description).IsUnique();

            builder.Entity<Review>().HasIndex(r => r.Title).IsUnique();
            builder.Entity<Review>().HasIndex(r => r.Text).IsUnique();

            builder.Entity<Career>().HasIndex(c => c.Name).IsUnique();

            builder.Entity<Skill>().HasIndex(s => s.Name).IsUnique();
        }

        public DbSet<Bootcamp> Bootcamps { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Career> Careers { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<ApplicationUser> Users { get; set; }
    }
}

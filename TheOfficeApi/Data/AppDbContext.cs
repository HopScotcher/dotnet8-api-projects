using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TheOfficeApi.DTOs;
using TheOfficeApi.Models;

namespace TheOfficeApi.Data
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options ) :base(options)
        {
        }
        public DbSet<Employee> Employees {get; set;}
        public DbSet<Department> Departments {get; set;}
        public DbSet<RefreshToken> RefreshTokens {get; set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Department>().HasData(
                new Department{Id = 1, Name = "Human resources", Code ="HR"},
                new Department{Id = 2, Name = "Sales", Code ="SALES"},
                new Department{Id = 3, Name = "Accounting", Code ="ACCT"},
                new Department{Id = 4, Name = "Warehouse", Code ="wH"}
            );
        }

       
    }
}
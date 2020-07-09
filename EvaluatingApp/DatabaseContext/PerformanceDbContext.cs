using EvaluatingApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace EvaluatingApp.DatabaseContext
{
    public class PerformanceDbContext : DbContext
    {
        public DbSet<UrlAddress> UrlAdresses { get; set; }

        public DbSet<Response> Responses { get; set; }

        public PerformanceDbContext()
        {
            this.Database.Migrate();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string conn = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            optionsBuilder.UseSqlServer(conn);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }
}
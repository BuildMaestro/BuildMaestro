using Microsoft.Data.Entity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using BuildMaestro.Data.Models;

namespace BuildMaestro.Data
{
    public class BuildMaestroContext : DbContext
    {
        public DbSet<Models.ApplicationSetting> AplicationSettings { get; set; }
        public DbSet<Models.BuildConfiguration> BuildConfigurations { get; set; }

        public string ConnectionString { get; set; }

        public BuildMaestroContext()
        {

        }
        public BuildMaestroContext(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (string.IsNullOrEmpty(this.ConnectionString))
            {
                var builder = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json");

                var connectionString = builder.Build().Get<string>("Data:ConnectionString");

                optionsBuilder.UseSqlServer(connectionString);
            }
            else
            {
                optionsBuilder.UseSqlServer(this.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                modelBuilder.Entity(entity.Name).ToTable(entity.Name + "s");
            }

            //one-to-many 
            modelBuilder.Entity<GitCommit>()
                        .HasOne<BuildConfiguration>(s => s.BuildConfiguration)
                        .WithMany(s => s.GitCommits)
                        .HasForeignKey(s => s.BuildConfigurationId);
        }
    }
}

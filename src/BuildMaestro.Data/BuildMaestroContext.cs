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
        public DbSet<Models.MsBuildConfiguration> MsBuildConfigurations { get; set; }
        public DbSet<Models.BuildConfiguration> RepositoryConfigurations { get; set; }

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

            modelBuilder.Entity<RepositoryConfiguration>()
                        .HasOne<BuildConfiguration>(s => s.BuildConfiguration)
                        .WithMany(s => s.RepositoryConfigurations)
                        .HasForeignKey(s => s.BuildConfigurationId);

            modelBuilder.Entity<GitCommit>()
                        .HasOne<RepositoryConfiguration>(s => s.RepositoryConfiguration)
                        .WithMany(s => s.GitCommits)
                        .HasForeignKey(s => s.RepositoryConfigurationId);

            modelBuilder.Entity<MsBuildConfiguration>()
                        .HasOne<BuildConfiguration>(s => s.BuildConfiguration)
                        .WithMany(x => x.MsBuildConfigurations)
                        .HasForeignKey(s => s.BuildConfigurationId);
        }
    }
}

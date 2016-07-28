using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations;
using BuildMaestro.Data;

namespace BuildMaestro.Data.Migrations
{
    [DbContext(typeof(BuildMaestroContext))]
    partial class BuildMaestroContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0-rc1-16348")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("BuildMaestro.Data.Models.ApplicationSetting", b =>
                {
                    b.Property<int>("Key");

                    b.Property<string>("Value");

                    b.HasKey("Key");

                    b.HasAnnotation("Relational:TableName", "BuildMaestro.Data.Models.ApplicationSettings");
                });

            modelBuilder.Entity("BuildMaestro.Data.Models.BuildConfiguration", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Active");

                    b.Property<bool>("Enabled");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:TableName", "BuildMaestro.Data.Models.BuildConfigurations");
                });

            modelBuilder.Entity("BuildMaestro.Data.Models.GitCommit", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Author");

                    b.Property<string>("Branch");

                    b.Property<DateTime>("DateTime");

                    b.Property<string>("Hash");

                    b.Property<string>("Message");

                    b.Property<string>("Repository");

                    b.Property<int>("RepositoryConfigurationId");

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:TableName", "BuildMaestro.Data.Models.GitCommits");
                });

            modelBuilder.Entity("BuildMaestro.Data.Models.MsBuildConfiguration", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Active");

                    b.Property<int>("BuildConfigurationId");

                    b.Property<int?>("RepositoryConfigurationId");

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:TableName", "BuildMaestro.Data.Models.MsBuildConfigurations");
                });

            modelBuilder.Entity("BuildMaestro.Data.Models.RepositoryConfiguration", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Active");

                    b.Property<bool>("AutoUpdate");

                    b.Property<string>("AutoUpdateTag");

                    b.Property<string>("Branch");

                    b.Property<int>("BuildConfigurationId");

                    b.Property<string>("RelativeSolutionFileLocation");

                    b.Property<string>("RepositoryUrl");

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:TableName", "BuildMaestro.Data.Models.RepositoryConfigurations");
                });

            modelBuilder.Entity("BuildMaestro.Data.Models.GitCommit", b =>
                {
                    b.HasOne("BuildMaestro.Data.Models.RepositoryConfiguration")
                        .WithMany()
                        .HasForeignKey("RepositoryConfigurationId");
                });

            modelBuilder.Entity("BuildMaestro.Data.Models.MsBuildConfiguration", b =>
                {
                    b.HasOne("BuildMaestro.Data.Models.BuildConfiguration")
                        .WithMany()
                        .HasForeignKey("BuildConfigurationId");

                    b.HasOne("BuildMaestro.Data.Models.RepositoryConfiguration")
                        .WithMany()
                        .HasForeignKey("RepositoryConfigurationId");
                });

            modelBuilder.Entity("BuildMaestro.Data.Models.RepositoryConfiguration", b =>
                {
                    b.HasOne("BuildMaestro.Data.Models.BuildConfiguration")
                        .WithMany()
                        .HasForeignKey("BuildConfigurationId");
                });
        }
    }
}

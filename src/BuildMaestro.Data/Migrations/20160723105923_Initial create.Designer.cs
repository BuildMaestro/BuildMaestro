using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations;
using BuildMaestro.Data;

namespace BuildMaestro.Data.Migrations
{
    [DbContext(typeof(BuildMaestroContext))]
    [Migration("20160723105923_Initial create")]
    partial class Initialcreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
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

                    b.Property<bool>("AutoDeploy");

                    b.Property<string>("AutoDeployTags");

                    b.Property<bool>("CleanWorkspace");

                    b.Property<bool>("Enabled");

                    b.Property<string>("GitBranch");

                    b.Property<string>("GitRepository");

                    b.Property<string>("Name");

                    b.Property<string>("RelativeSolutionFileLocation");

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:TableName", "BuildMaestro.Data.Models.BuildConfigurations");
                });

            modelBuilder.Entity("BuildMaestro.Data.Models.GitCommit", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Author");

                    b.Property<string>("Branch");

                    b.Property<int>("BuildConfigurationId");

                    b.Property<DateTime>("DateTime");

                    b.Property<string>("Hash");

                    b.Property<string>("Message");

                    b.Property<string>("Repository");

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:TableName", "BuildMaestro.Data.Models.GitCommits");
                });

            modelBuilder.Entity("BuildMaestro.Data.Models.GitCommit", b =>
                {
                    b.HasOne("BuildMaestro.Data.Models.BuildConfiguration")
                        .WithMany()
                        .HasForeignKey("BuildConfigurationId");
                });
        }
    }
}

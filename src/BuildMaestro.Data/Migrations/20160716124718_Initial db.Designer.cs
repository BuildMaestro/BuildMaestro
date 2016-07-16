using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations;
using BuildMaestro.Data;

namespace BuildMaestro.Data.Migrations
{
    [DbContext(typeof(BuildMaestroContext))]
    [Migration("20160716124718_Initial db")]
    partial class Initialdb
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0-rc1-16348")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("BuildMaestro.Models.BuildConfiguration", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("AutoDeploy");

                    b.Property<string>("AutoDeployTags");

                    b.Property<bool>("Enabled");

                    b.Property<string>("GitBranch");

                    b.Property<DateTime?>("GitCommitDate");

                    b.Property<string>("GitCommitHash");

                    b.Property<string>("GitRepository");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:TableName", "BuildMaestro.Models.BuildConfigurations");
                });
        }
    }
}

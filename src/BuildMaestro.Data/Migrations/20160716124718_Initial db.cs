using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;
using Microsoft.Data.Entity.Metadata;

namespace BuildMaestro.Data.Migrations
{
    public partial class Initialdb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BuildMaestro.Models.BuildConfigurations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AutoDeploy = table.Column<bool>(nullable: false),
                    AutoDeployTags = table.Column<string>(nullable: true),
                    Enabled = table.Column<bool>(nullable: false),
                    GitBranch = table.Column<string>(nullable: true),
                    GitCommitDate = table.Column<DateTime>(nullable: true),
                    GitCommitHash = table.Column<string>(nullable: true),
                    GitRepository = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuildConfiguration", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable("BuildMaestro.Models.BuildConfigurations");
        }
    }
}

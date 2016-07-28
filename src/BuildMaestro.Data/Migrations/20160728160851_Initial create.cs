using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;
using Microsoft.Data.Entity.Metadata;

namespace BuildMaestro.Data.Migrations
{
    public partial class Initialcreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BuildMaestro.Data.Models.ApplicationSettings",
                columns: table => new
                {
                    Key = table.Column<int>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationSetting", x => x.Key);
                });
            migrationBuilder.CreateTable(
                name: "BuildMaestro.Data.Models.BuildConfigurations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(nullable: false),
                    Enabled = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuildConfiguration", x => x.Id);
                });
            migrationBuilder.CreateTable(
                name: "BuildMaestro.Data.Models.RepositoryConfigurations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(nullable: false),
                    AutoUpdate = table.Column<bool>(nullable: false),
                    AutoUpdateTag = table.Column<string>(nullable: true),
                    Branch = table.Column<string>(nullable: true),
                    BuildConfigurationId = table.Column<int>(nullable: false),
                    RelativeSolutionFileLocation = table.Column<string>(nullable: true),
                    RepositoryUrl = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RepositoryConfiguration", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RepositoryConfiguration_BuildConfiguration_BuildConfigurationId",
                        column: x => x.BuildConfigurationId,
                        principalTable: "BuildMaestro.Data.Models.BuildConfigurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.CreateTable(
                name: "BuildMaestro.Data.Models.GitCommits",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Author = table.Column<string>(nullable: true),
                    Branch = table.Column<string>(nullable: true),
                    DateTime = table.Column<DateTime>(nullable: false),
                    Hash = table.Column<string>(nullable: true),
                    Message = table.Column<string>(nullable: true),
                    Repository = table.Column<string>(nullable: true),
                    RepositoryConfigurationId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GitCommit", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GitCommit_RepositoryConfiguration_RepositoryConfigurationId",
                        column: x => x.RepositoryConfigurationId,
                        principalTable: "BuildMaestro.Data.Models.RepositoryConfigurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.CreateTable(
                name: "BuildMaestro.Data.Models.MsBuildConfigurations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(nullable: false),
                    BuildConfigurationId = table.Column<int>(nullable: false),
                    RepositoryConfigurationId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MsBuildConfiguration", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MsBuildConfiguration_BuildConfiguration_BuildConfigurationId",
                        column: x => x.BuildConfigurationId,
                        principalTable: "BuildMaestro.Data.Models.BuildConfigurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MsBuildConfiguration_RepositoryConfiguration_RepositoryConfigurationId",
                        column: x => x.RepositoryConfigurationId,
                        principalTable: "BuildMaestro.Data.Models.RepositoryConfigurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable("BuildMaestro.Data.Models.ApplicationSettings");
            migrationBuilder.DropTable("BuildMaestro.Data.Models.GitCommits");
            migrationBuilder.DropTable("BuildMaestro.Data.Models.MsBuildConfigurations");
            migrationBuilder.DropTable("BuildMaestro.Data.Models.RepositoryConfigurations");
            migrationBuilder.DropTable("BuildMaestro.Data.Models.BuildConfigurations");
        }
    }
}

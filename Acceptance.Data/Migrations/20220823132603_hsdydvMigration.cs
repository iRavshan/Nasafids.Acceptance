using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Acceptance.Data.Migrations
{
    public partial class hsdydvMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Faculty",
                table: "Applicants");

            migrationBuilder.AddColumn<Guid>(
                name: "FacultyId",
                table: "Applicants",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Faculties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Number = table.Column<int>(type: "int", nullable: false),
                    PriceOfDay = table.Column<int>(type: "int", nullable: false),
                    PriceOfNight = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Faculties", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Applicants_FacultyId",
                table: "Applicants",
                column: "FacultyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Applicants_Faculties_FacultyId",
                table: "Applicants",
                column: "FacultyId",
                principalTable: "Faculties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applicants_Faculties_FacultyId",
                table: "Applicants");

            migrationBuilder.DropTable(
                name: "Faculties");

            migrationBuilder.DropIndex(
                name: "IX_Applicants_FacultyId",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "FacultyId",
                table: "Applicants");

            migrationBuilder.AddColumn<string>(
                name: "Faculty",
                table: "Applicants",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}

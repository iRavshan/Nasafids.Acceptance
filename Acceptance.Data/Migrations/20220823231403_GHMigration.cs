using Microsoft.EntityFrameworkCore.Migrations;

namespace Acceptance.Data.Migrations
{
    public partial class GHMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PhotoUrl",
                table: "Applicants",
                newName: "SecondPhoneNumber");

            migrationBuilder.RenameColumn(
                name: "PassportImageUrl",
                table: "Applicants",
                newName: "PassportSeries");

            migrationBuilder.RenameColumn(
                name: "DiplomaImageUrl",
                table: "Applicants",
                newName: "PassportNumber");

            migrationBuilder.AddColumn<string>(
                name: "JShIR",
                table: "Applicants",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Applicants",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JShIR",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Applicants");

            migrationBuilder.RenameColumn(
                name: "SecondPhoneNumber",
                table: "Applicants",
                newName: "PhotoUrl");

            migrationBuilder.RenameColumn(
                name: "PassportSeries",
                table: "Applicants",
                newName: "PassportImageUrl");

            migrationBuilder.RenameColumn(
                name: "PassportNumber",
                table: "Applicants",
                newName: "DiplomaImageUrl");
        }
    }
}

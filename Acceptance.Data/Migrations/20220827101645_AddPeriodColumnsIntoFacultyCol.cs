using Microsoft.EntityFrameworkCore.Migrations;

namespace Acceptance.Data.Migrations
{
    public partial class AddPeriodColumnsIntoFacultyCol : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PeriodOfDay",
                table: "Faculties",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PeriodOfNight",
                table: "Faculties",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PeriodOfDay",
                table: "Faculties");

            migrationBuilder.DropColumn(
                name: "PeriodOfNight",
                table: "Faculties");
        }
    }
}

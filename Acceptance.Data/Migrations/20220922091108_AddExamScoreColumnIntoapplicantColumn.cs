using Microsoft.EntityFrameworkCore.Migrations;

namespace Acceptance.Data.Migrations
{
    public partial class AddExamScoreColumnIntoapplicantColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "ExamScrore",
                table: "Applicants",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExamScrore",
                table: "Applicants");
        }
    }
}

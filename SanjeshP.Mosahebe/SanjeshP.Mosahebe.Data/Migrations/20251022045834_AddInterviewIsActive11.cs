using Microsoft.EntityFrameworkCore.Migrations;

namespace SanjeshP.Mosahebe.Data.Migrations
{
    public partial class AddInterviewIsActive11 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InterviewIsActive",
                table: "Students");

            migrationBuilder.AddColumn<int>(
                name: "InterviewProcessStatus",
                table: "Students",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InterviewProcessStatus",
                table: "Students");

            migrationBuilder.AddColumn<int>(
                name: "InterviewIsActive",
                table: "Students",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}

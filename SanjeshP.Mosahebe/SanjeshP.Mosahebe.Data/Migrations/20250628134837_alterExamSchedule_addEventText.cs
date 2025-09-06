using Microsoft.EntityFrameworkCore.Migrations;

namespace SanjeshP.Mosahebe.Data.Migrations
{
    public partial class alterExamSchedule_addEventText : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EventTypeDisplayName",
                table: "ExamSchedules",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EventTypeName",
                table: "ExamSchedules",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EventTypeDisplayName",
                table: "ExamSchedules");

            migrationBuilder.DropColumn(
                name: "EventTypeName",
                table: "ExamSchedules");
        }
    }
}

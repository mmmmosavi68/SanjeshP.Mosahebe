using Microsoft.EntityFrameworkCore.Migrations;

namespace SanjeshP.Mosahebe.Data.Migrations
{
    public partial class alter_addResearchDocumentScore : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExamCourseType",
                table: "Students");

            migrationBuilder.AddColumn<int>(
                name: "IsPajoheshi",
                table: "Users",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "Students",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectReason",
                table: "Students",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IsPajoheshi",
                table: "ExamFields",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPajoheshi",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "RejectReason",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "IsPajoheshi",
                table: "ExamFields");

            migrationBuilder.AddColumn<int>(
                name: "ExamCourseType",
                table: "Students",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}

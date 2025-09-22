using Microsoft.EntityFrameworkCore.Migrations;

namespace SanjeshP.Mosahebe.Data.Migrations
{
    public partial class AlterExamField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "ExamFields");

            migrationBuilder.DropColumn(
                name: "IsPajoheshi",
                table: "ExamFields");

            migrationBuilder.AddColumn<int>(
                name: "StandardCode",
                table: "ExamFields",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StandardCode",
                table: "ExamFields");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "ExamFields",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IsPajoheshi",
                table: "ExamFields",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}

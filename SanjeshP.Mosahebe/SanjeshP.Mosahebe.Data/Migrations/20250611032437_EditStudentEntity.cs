using Microsoft.EntityFrameworkCore.Migrations;

namespace SanjeshP.Mosahebe.Data.Migrations
{
    public partial class EditStudentEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Dore",
                table: "ArchiveData");

            migrationBuilder.DropColumn(
                name: "Sal",
                table: "ArchiveData");

            migrationBuilder.AddColumn<string>(
                name: "Term",
                table: "ArchiveData",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "ArchiveData",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Term",
                table: "ArchiveData");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "ArchiveData");

            migrationBuilder.AddColumn<string>(
                name: "Dore",
                table: "ArchiveData",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Sal",
                table: "ArchiveData",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace SanjeshP.Mosahebe.Data.Migrations
{
    public partial class AlterCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PrimaryCode",
                table: "Codes",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SubCode",
                table: "Codes",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrimaryCode",
                table: "Codes");

            migrationBuilder.DropColumn(
                name: "SubCode",
                table: "Codes");
        }
    }
}

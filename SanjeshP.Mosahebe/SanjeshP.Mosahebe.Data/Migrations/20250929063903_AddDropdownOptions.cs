using Microsoft.EntityFrameworkCore.Migrations;

namespace SanjeshP.Mosahebe.Data.Migrations
{
    public partial class AddDropdownOptions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DropdownOptions",
                table: "FormItemDefinitions",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DropdownOptions",
                table: "FormItemDefinitions");
        }
    }
}

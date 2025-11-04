using Microsoft.EntityFrameworkCore.Migrations;

namespace SanjeshP.Mosahebe.Data.Migrations
{
    public partial class AlterFormCategoriesFormItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaxCount",
                table: "FormItems",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "FormItemCategories",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxCount",
                table: "FormItems");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "FormItemCategories");
        }
    }
}

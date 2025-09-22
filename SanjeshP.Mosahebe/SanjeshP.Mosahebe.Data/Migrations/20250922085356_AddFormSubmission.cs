using Microsoft.EntityFrameworkCore.Migrations;

namespace SanjeshP.Mosahebe.Data.Migrations
{
    public partial class AddFormSubmission : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JsonData",
                table: "FormSubmissions");

            migrationBuilder.AddColumn<int>(
                name: "FieldDefinitionId",
                table: "FormSubmissions",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Value",
                table: "FormSubmissions",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FieldDefinitionId",
                table: "FormSubmissions");

            migrationBuilder.DropColumn(
                name: "Value",
                table: "FormSubmissions");

            migrationBuilder.AddColumn<string>(
                name: "JsonData",
                table: "FormSubmissions",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}

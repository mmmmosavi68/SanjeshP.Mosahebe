using Microsoft.EntityFrameworkCore.Migrations;

namespace SanjeshP.Mosahebe.Data.Migrations
{
    public partial class fdsf : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "FormItemDefinitionId",
                table: "StudentFormSubmissionData",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "FormItemCoefficientDefinitionId",
                table: "StudentFormSubmissionData",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "FormItemDefinitionId",
                table: "StudentFormSubmissionData",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "FormItemCoefficientDefinitionId",
                table: "StudentFormSubmissionData",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}

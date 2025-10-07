using Microsoft.EntityFrameworkCore.Migrations;

namespace SanjeshP.Mosahebe.Data.Migrations
{
    public partial class dddddd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FormItemCoefficientDefinitionId",
                table: "Coefficients",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Coefficients_FormItemCoefficientDefinitionId",
                table: "Coefficients",
                column: "FormItemCoefficientDefinitionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Coefficients_FormItemCoefficientDefinitions_FormItemCoefficientDefinitionId",
                table: "Coefficients",
                column: "FormItemCoefficientDefinitionId",
                principalTable: "FormItemCoefficientDefinitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Coefficients_FormItemCoefficientDefinitions_FormItemCoefficientDefinitionId",
                table: "Coefficients");

            migrationBuilder.DropIndex(
                name: "IX_Coefficients_FormItemCoefficientDefinitionId",
                table: "Coefficients");

            migrationBuilder.DropColumn(
                name: "FormItemCoefficientDefinitionId",
                table: "Coefficients");
        }
    }
}

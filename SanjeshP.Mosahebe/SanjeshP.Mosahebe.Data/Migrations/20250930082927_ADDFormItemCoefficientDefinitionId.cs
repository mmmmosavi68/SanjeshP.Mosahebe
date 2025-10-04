using Microsoft.EntityFrameworkCore.Migrations;

namespace SanjeshP.Mosahebe.Data.Migrations
{
    public partial class ADDFormItemCoefficientDefinitionId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FormItemCoefficientDefinitionId",
                table: "StudentFormSubmissionData",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_StudentFormSubmissionData_FormItemCoefficientDefinitionId",
                table: "StudentFormSubmissionData",
                column: "FormItemCoefficientDefinitionId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentFormSubmissionData_FormItemCoefficientDefinitions_FormItemCoefficientDefinitionId",
                table: "StudentFormSubmissionData",
                column: "FormItemCoefficientDefinitionId",
                principalTable: "FormItemCoefficientDefinitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentFormSubmissionData_FormItemCoefficientDefinitions_FormItemCoefficientDefinitionId",
                table: "StudentFormSubmissionData");

            migrationBuilder.DropIndex(
                name: "IX_StudentFormSubmissionData_FormItemCoefficientDefinitionId",
                table: "StudentFormSubmissionData");

            migrationBuilder.DropColumn(
                name: "FormItemCoefficientDefinitionId",
                table: "StudentFormSubmissionData");
        }
    }
}

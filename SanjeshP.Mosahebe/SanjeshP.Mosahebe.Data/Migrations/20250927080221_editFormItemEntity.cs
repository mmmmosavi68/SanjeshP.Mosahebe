using Microsoft.EntityFrameworkCore.Migrations;

namespace SanjeshP.Mosahebe.Data.Migrations
{
    public partial class editFormItemEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FormItemDefinitions_FormItemCoefficients_FormItemTypeDefaultId",
                table: "FormItemDefinitions");

            migrationBuilder.DropForeignKey(
                name: "FK_FormItems_FormItemCoefficients_FormItemTypeId",
                table: "FormItems");

            migrationBuilder.DropIndex(
                name: "IX_FormItems_FormItemTypeId",
                table: "FormItems");

            migrationBuilder.DropIndex(
                name: "IX_FormItemDefinitions_FormItemTypeDefaultId",
                table: "FormItemDefinitions");

            migrationBuilder.DropColumn(
                name: "FormItemTypeId",
                table: "FormItems");

            migrationBuilder.DropColumn(
                name: "FormItemTypeDefaultId",
                table: "FormItemDefinitions");

            migrationBuilder.AddColumn<int>(
                name: "FormItemCoefficientId",
                table: "FormItems",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_FormItems_FormItemCoefficientId",
                table: "FormItems",
                column: "FormItemCoefficientId");

            migrationBuilder.AddForeignKey(
                name: "FK_FormItems_FormItemCoefficients_FormItemCoefficientId",
                table: "FormItems",
                column: "FormItemCoefficientId",
                principalTable: "FormItemCoefficients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FormItems_FormItemCoefficients_FormItemCoefficientId",
                table: "FormItems");

            migrationBuilder.DropIndex(
                name: "IX_FormItems_FormItemCoefficientId",
                table: "FormItems");

            migrationBuilder.DropColumn(
                name: "FormItemCoefficientId",
                table: "FormItems");

            migrationBuilder.AddColumn<int>(
                name: "FormItemTypeId",
                table: "FormItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FormItemTypeDefaultId",
                table: "FormItemDefinitions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_FormItems_FormItemTypeId",
                table: "FormItems",
                column: "FormItemTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_FormItemDefinitions_FormItemTypeDefaultId",
                table: "FormItemDefinitions",
                column: "FormItemTypeDefaultId");

            migrationBuilder.AddForeignKey(
                name: "FK_FormItemDefinitions_FormItemCoefficients_FormItemTypeDefaultId",
                table: "FormItemDefinitions",
                column: "FormItemTypeDefaultId",
                principalTable: "FormItemCoefficients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FormItems_FormItemCoefficients_FormItemTypeId",
                table: "FormItems",
                column: "FormItemTypeId",
                principalTable: "FormItemCoefficients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

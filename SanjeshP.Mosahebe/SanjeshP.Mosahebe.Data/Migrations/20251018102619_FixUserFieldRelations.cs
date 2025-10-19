using Microsoft.EntityFrameworkCore.Migrations;

namespace SanjeshP.Mosahebe.Data.Migrations
{
    public partial class FixUserFieldRelations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExamFields_UserFields_UserFieldId",
                table: "ExamFields");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_UserFields_UserFieldId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_UserFieldId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_ExamFields_UserFieldId",
                table: "ExamFields");

            migrationBuilder.DropColumn(
                name: "UserFieldId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UserFieldId",
                table: "ExamFields");

            migrationBuilder.CreateIndex(
                name: "IX_UserFields_ExamFieldID",
                table: "UserFields",
                column: "ExamFieldID");

            migrationBuilder.CreateIndex(
                name: "IX_UserFields_UserID",
                table: "UserFields",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_UserFields_ExamFields_ExamFieldID",
                table: "UserFields",
                column: "ExamFieldID",
                principalTable: "ExamFields",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserFields_Users_UserID",
                table: "UserFields",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserFields_ExamFields_ExamFieldID",
                table: "UserFields");

            migrationBuilder.DropForeignKey(
                name: "FK_UserFields_Users_UserID",
                table: "UserFields");

            migrationBuilder.DropIndex(
                name: "IX_UserFields_ExamFieldID",
                table: "UserFields");

            migrationBuilder.DropIndex(
                name: "IX_UserFields_UserID",
                table: "UserFields");

            migrationBuilder.AddColumn<int>(
                name: "UserFieldId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserFieldId",
                table: "ExamFields",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserFieldId",
                table: "Users",
                column: "UserFieldId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamFields_UserFieldId",
                table: "ExamFields",
                column: "UserFieldId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExamFields_UserFields_UserFieldId",
                table: "ExamFields",
                column: "UserFieldId",
                principalTable: "UserFields",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_UserFields_UserFieldId",
                table: "Users",
                column: "UserFieldId",
                principalTable: "UserFields",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

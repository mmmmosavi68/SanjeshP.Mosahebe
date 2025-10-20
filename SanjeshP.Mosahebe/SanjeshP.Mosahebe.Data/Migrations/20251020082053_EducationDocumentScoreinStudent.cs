using Microsoft.EntityFrameworkCore.Migrations;

namespace SanjeshP.Mosahebe.Data.Migrations
{
    public partial class EducationDocumentScoreinStudent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_EducationDocumentScores_StudentId",
                table: "EducationDocumentScores",
                column: "StudentId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_EducationDocumentScores_Students_StudentId",
                table: "EducationDocumentScores",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EducationDocumentScores_Students_StudentId",
                table: "EducationDocumentScores");

            migrationBuilder.DropIndex(
                name: "IX_EducationDocumentScores_StudentId",
                table: "EducationDocumentScores");
        }
    }
}

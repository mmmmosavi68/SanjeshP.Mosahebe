using Microsoft.EntityFrameworkCore.Migrations;

namespace SanjeshP.Mosahebe.Data.Migrations
{
    public partial class alterStudewnt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EducationDocumentScores_StudentId",
                table: "EducationDocumentScores");

            migrationBuilder.CreateIndex(
                name: "IX_EducationDocumentScores_StudentId",
                table: "EducationDocumentScores",
                column: "StudentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EducationDocumentScores_StudentId",
                table: "EducationDocumentScores");

            migrationBuilder.CreateIndex(
                name: "IX_EducationDocumentScores_StudentId",
                table: "EducationDocumentScores",
                column: "StudentId",
                unique: true);
        }
    }
}

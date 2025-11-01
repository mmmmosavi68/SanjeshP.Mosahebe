using Microsoft.EntityFrameworkCore.Migrations;

namespace SanjeshP.Mosahebe.Data.Migrations
{
    public partial class alterIsapproved : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ResearchDocumentScores_StudentId",
                table: "ResearchDocumentScores");

            migrationBuilder.AlterColumn<int>(
                name: "IsApproved",
                table: "ResearchDocumentScores",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ResearchDocumentScores_StudentId",
                table: "ResearchDocumentScores",
                column: "StudentId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ResearchDocumentScores_StudentId",
                table: "ResearchDocumentScores");

            migrationBuilder.AlterColumn<bool>(
                name: "IsApproved",
                table: "ResearchDocumentScores",
                type: "bit",
                nullable: true,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ResearchDocumentScores_StudentId",
                table: "ResearchDocumentScores",
                column: "StudentId");
        }
    }
}

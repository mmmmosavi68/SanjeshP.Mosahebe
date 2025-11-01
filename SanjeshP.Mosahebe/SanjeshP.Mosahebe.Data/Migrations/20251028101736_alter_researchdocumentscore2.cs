using Microsoft.EntityFrameworkCore.Migrations;

namespace SanjeshP.Mosahebe.Data.Migrations
{
    public partial class alter_researchdocumentscore2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "ResearchDocumentScores",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectReason",
                table: "ResearchDocumentScores",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "ResearchDocumentScores");

            migrationBuilder.DropColumn(
                name: "RejectReason",
                table: "ResearchDocumentScores");
        }
    }
}

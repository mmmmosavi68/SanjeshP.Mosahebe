using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SanjeshP.Mosahebe.Data.Migrations
{
    public partial class addStudentFormSubmitionScore : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "StudentFormSubmissions");

            migrationBuilder.DropColumn(
                name: "RejectReason",
                table: "StudentFormSubmissions");

            migrationBuilder.DropColumn(
                name: "Score",
                table: "StudentFormSubmissions");

            migrationBuilder.CreateTable(
                name: "StudentFormSubmissionScores",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<Guid>(nullable: false),
                    HostIp = table.Column<string>(nullable: true),
                    StudentFormSubmissionId = table.Column<int>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false),
                    StudentId = table.Column<int>(nullable: false),
                    IsApproved = table.Column<bool>(nullable: false),
                    RejectReason = table.Column<string>(nullable: true),
                    UserScore = table.Column<decimal>(type: "decimal(6,3)", nullable: true),
                    CalculatedScore = table.Column<decimal>(type: "decimal(6,3)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentFormSubmissionScores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentFormSubmissionScores_StudentFormSubmissions_StudentFormSubmissionId",
                        column: x => x.StudentFormSubmissionId,
                        principalTable: "StudentFormSubmissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentFormSubmissionScores_StudentFormSubmissionId",
                table: "StudentFormSubmissionScores",
                column: "StudentFormSubmissionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentFormSubmissionScores");

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "StudentFormSubmissions",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectReason",
                table: "StudentFormSubmissions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Score",
                table: "StudentFormSubmissions",
                type: "decimal(6,3)",
                nullable: true);
        }
    }
}

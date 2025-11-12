using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SanjeshP.Mosahebe.Data.Migrations
{
    public partial class EditStudentFormSubmition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentFormSubmissionData_StudentFormSubmissionScores_StudentFormSubmissionScoreId",
                table: "StudentFormSubmissionData");

            migrationBuilder.DropTable(
                name: "StudentFormSubmissionScores");

            migrationBuilder.DropIndex(
                name: "IX_StudentFormSubmissionData_StudentFormSubmissionScoreId",
                table: "StudentFormSubmissionData");

            migrationBuilder.DropColumn(
                name: "StudentFormSubmissionScoreId",
                table: "UploadedFiles");

            migrationBuilder.DropColumn(
                name: "StudentFormSubmissionScoreId",
                table: "StudentFormSubmissionData");

            migrationBuilder.AddColumn<int>(
                name: "StudentFormSubmissionId",
                table: "UploadedFiles",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StudentFormSubmissionId",
                table: "StudentFormSubmissionData",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "StudentFormSubmissions",
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
                    FormItemId = table.Column<int>(nullable: false),
                    StudentId = table.Column<int>(nullable: true),
                    Score = table.Column<decimal>(type: "decimal(6,3)", nullable: true),
                    IsApproved = table.Column<bool>(nullable: true),
                    RejectReason = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentFormSubmissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentFormSubmissions_FormItems_FormItemId",
                        column: x => x.FormItemId,
                        principalTable: "FormItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentFormSubmissionData_StudentFormSubmissionId",
                table: "StudentFormSubmissionData",
                column: "StudentFormSubmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentFormSubmissions_FormItemId",
                table: "StudentFormSubmissions",
                column: "FormItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentFormSubmissionData_StudentFormSubmissions_StudentFormSubmissionId",
                table: "StudentFormSubmissionData",
                column: "StudentFormSubmissionId",
                principalTable: "StudentFormSubmissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentFormSubmissionData_StudentFormSubmissions_StudentFormSubmissionId",
                table: "StudentFormSubmissionData");

            migrationBuilder.DropTable(
                name: "StudentFormSubmissions");

            migrationBuilder.DropIndex(
                name: "IX_StudentFormSubmissionData_StudentFormSubmissionId",
                table: "StudentFormSubmissionData");

            migrationBuilder.DropColumn(
                name: "StudentFormSubmissionId",
                table: "UploadedFiles");

            migrationBuilder.DropColumn(
                name: "StudentFormSubmissionId",
                table: "StudentFormSubmissionData");

            migrationBuilder.AddColumn<int>(
                name: "StudentFormSubmissionScoreId",
                table: "UploadedFiles",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StudentFormSubmissionScoreId",
                table: "StudentFormSubmissionData",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "StudentFormSubmissionScores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FormItemId = table.Column<int>(type: "int", nullable: false),
                    HostIp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    RejectReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Score = table.Column<decimal>(type: "decimal(6,3)", nullable: true),
                    StudentId = table.Column<int>(type: "int", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentFormSubmissionScores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentFormSubmissionScores_FormItems_FormItemId",
                        column: x => x.FormItemId,
                        principalTable: "FormItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentFormSubmissionData_StudentFormSubmissionScoreId",
                table: "StudentFormSubmissionData",
                column: "StudentFormSubmissionScoreId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentFormSubmissionScores_FormItemId",
                table: "StudentFormSubmissionScores",
                column: "FormItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentFormSubmissionData_StudentFormSubmissionScores_StudentFormSubmissionScoreId",
                table: "StudentFormSubmissionData",
                column: "StudentFormSubmissionScoreId",
                principalTable: "StudentFormSubmissionScores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

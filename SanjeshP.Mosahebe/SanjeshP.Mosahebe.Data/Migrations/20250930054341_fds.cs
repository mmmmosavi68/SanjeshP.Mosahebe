using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SanjeshP.Mosahebe.Data.Migrations
{
    public partial class fds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentFormSubmissionData_StudentFormSubmissionScores_FormSubmissionId",
                table: "StudentFormSubmissionData");

            migrationBuilder.DropTable(
                name: "FieldDefinitions");

            migrationBuilder.DropTable(
                name: "FormSubmissions");

            migrationBuilder.DropIndex(
                name: "IX_StudentFormSubmissionData_FormSubmissionId",
                table: "StudentFormSubmissionData");

            migrationBuilder.DropColumn(
                name: "FormSubmissionId",
                table: "StudentFormSubmissionData");

            migrationBuilder.AddColumn<int>(
                name: "StudentFormSubmissionScoreId",
                table: "StudentFormSubmissionData",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_StudentFormSubmissionData_StudentFormSubmissionScoreId",
                table: "StudentFormSubmissionData",
                column: "StudentFormSubmissionScoreId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentFormSubmissionData_StudentFormSubmissionScores_StudentFormSubmissionScoreId",
                table: "StudentFormSubmissionData",
                column: "StudentFormSubmissionScoreId",
                principalTable: "StudentFormSubmissionScores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentFormSubmissionData_StudentFormSubmissionScores_StudentFormSubmissionScoreId",
                table: "StudentFormSubmissionData");

            migrationBuilder.DropIndex(
                name: "IX_StudentFormSubmissionData_StudentFormSubmissionScoreId",
                table: "StudentFormSubmissionData");

            migrationBuilder.DropColumn(
                name: "StudentFormSubmissionScoreId",
                table: "StudentFormSubmissionData");

            migrationBuilder.AddColumn<int>(
                name: "FormSubmissionId",
                table: "StudentFormSubmissionData",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "FieldDefinitions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FormId = table.Column<int>(type: "int", nullable: false),
                    HostIp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    Label = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    TypeText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldDefinitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FormSubmissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FieldDefinitionId = table.Column<int>(type: "int", nullable: false),
                    FormId = table.Column<int>(type: "int", nullable: false),
                    HostIp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormSubmissions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentFormSubmissionData_FormSubmissionId",
                table: "StudentFormSubmissionData",
                column: "FormSubmissionId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentFormSubmissionData_StudentFormSubmissionScores_FormSubmissionId",
                table: "StudentFormSubmissionData",
                column: "FormSubmissionId",
                principalTable: "StudentFormSubmissionScores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

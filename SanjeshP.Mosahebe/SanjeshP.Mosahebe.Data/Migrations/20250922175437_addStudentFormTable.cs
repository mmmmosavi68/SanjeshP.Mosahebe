using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SanjeshP.Mosahebe.Data.Migrations
{
    public partial class addStudentFormTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StudentForms",
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
                    Title = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentForms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StudentFieldDefinitions",
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
                    FormId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Label = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    IsRequired = table.Column<bool>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    ValidationPattern = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentFieldDefinitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentFieldDefinitions_StudentForms_FormId",
                        column: x => x.FormId,
                        principalTable: "StudentForms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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
                    FormId = table.Column<int>(nullable: false),
                    StudentId = table.Column<int>(nullable: true),
                    Score = table.Column<int>(nullable: true),
                    IsApproved = table.Column<bool>(nullable: true),
                    RejectReason = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentFormSubmissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentFormSubmissions_StudentForms_FormId",
                        column: x => x.FormId,
                        principalTable: "StudentForms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StudentFormSubmissionValues",
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
                    FormSubmissionId = table.Column<int>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    FieldDefinitionId = table.Column<int>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentFormSubmissionValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentFormSubmissionValues_StudentFieldDefinitions_FieldDefinitionId",
                        column: x => x.FieldDefinitionId,
                        principalTable: "StudentFieldDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentFormSubmissionValues_StudentFormSubmissions_FormSubmissionId",
                        column: x => x.FormSubmissionId,
                        principalTable: "StudentFormSubmissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentFieldDefinitions_FormId",
                table: "StudentFieldDefinitions",
                column: "FormId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentFormSubmissions_FormId",
                table: "StudentFormSubmissions",
                column: "FormId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentFormSubmissionValues_FieldDefinitionId",
                table: "StudentFormSubmissionValues",
                column: "FieldDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentFormSubmissionValues_FormSubmissionId",
                table: "StudentFormSubmissionValues",
                column: "FormSubmissionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentFormSubmissionValues");

            migrationBuilder.DropTable(
                name: "StudentFieldDefinitions");

            migrationBuilder.DropTable(
                name: "StudentFormSubmissions");

            migrationBuilder.DropTable(
                name: "StudentForms");
        }
    }
}

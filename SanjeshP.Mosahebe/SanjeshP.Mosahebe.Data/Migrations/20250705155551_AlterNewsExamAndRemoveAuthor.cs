using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace SanjeshP.Mosahebe.Data.Migrations
{
    public partial class AlterNewsExamAndRemoveAuthor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExamNews_Authors_AuthorId1",
                table: "ExamNews");

            migrationBuilder.DropForeignKey(
                name: "FK_ExamNews_Authors_ReviewedById1",
                table: "ExamNews");

            migrationBuilder.DropTable(
                name: "Authors");

            migrationBuilder.DropIndex(
                name: "IX_ExamNews_AuthorId1",
                table: "ExamNews");

            migrationBuilder.DropIndex(
                name: "IX_ExamNews_ReviewedById1",
                table: "ExamNews");

            migrationBuilder.DropColumn(
                name: "AuthorId1",
                table: "ExamNews");

            migrationBuilder.DropColumn(
                name: "ReviewedById1",
                table: "ExamNews");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AuthorId1",
                table: "ExamNews",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReviewedById1",
                table: "ExamNews",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Authors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Bio = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    HostIp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ProfileImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authors", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExamNews_AuthorId1",
                table: "ExamNews",
                column: "AuthorId1");

            migrationBuilder.CreateIndex(
                name: "IX_ExamNews_ReviewedById1",
                table: "ExamNews",
                column: "ReviewedById1");

            migrationBuilder.AddForeignKey(
                name: "FK_ExamNews_Authors_AuthorId1",
                table: "ExamNews",
                column: "AuthorId1",
                principalTable: "Authors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ExamNews_Authors_ReviewedById1",
                table: "ExamNews",
                column: "ReviewedById1",
                principalTable: "Authors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

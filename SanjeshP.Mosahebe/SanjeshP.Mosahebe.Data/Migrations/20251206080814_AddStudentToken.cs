using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SanjeshP.Mosahebe.Data.Migrations
{
    public partial class AddStudentToken : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "StudentTokenId",
                table: "StudentOtps",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "StudentId",
                table: "StudentFormSubmissions",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "StudentTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<Guid>(nullable: false),
                    HostIp = table.Column<string>(nullable: true),
                    StudentId = table.Column<int>(nullable: false),
                    SessionId = table.Column<string>(maxLength: 128, nullable: false),
                    UserAgent = table.Column<string>(maxLength: 256, nullable: true),
                    ExpirationDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentTokens_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentOtps_StudentTokenId",
                table: "StudentOtps",
                column: "StudentTokenId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentTokens_StudentId",
                table: "StudentTokens",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentOtps_StudentTokens_StudentTokenId",
                table: "StudentOtps",
                column: "StudentTokenId",
                principalTable: "StudentTokens",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentOtps_StudentTokens_StudentTokenId",
                table: "StudentOtps");

            migrationBuilder.DropTable(
                name: "StudentTokens");

            migrationBuilder.DropIndex(
                name: "IX_StudentOtps_StudentTokenId",
                table: "StudentOtps");

            migrationBuilder.DropColumn(
                name: "StudentTokenId",
                table: "StudentOtps");

            migrationBuilder.AlterColumn<int>(
                name: "StudentId",
                table: "StudentFormSubmissions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));
        }
    }
}

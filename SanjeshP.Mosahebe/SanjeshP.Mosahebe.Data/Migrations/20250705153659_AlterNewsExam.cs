using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace SanjeshP.Mosahebe.Data.Migrations
{
    public partial class AlterNewsExam : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "ExamNews");

            migrationBuilder.DropColumn(
                name: "PublishDate",
                table: "ExamNews");

            migrationBuilder.AddColumn<Guid>(
                name: "ExamID",
                table: "ExamNews",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TypeOfExamID",
                table: "ExamNews",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExamID",
                table: "ExamNews");

            migrationBuilder.DropColumn(
                name: "TypeOfExamID",
                table: "ExamNews");

            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId",
                table: "ExamNews",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "PublishDate",
                table: "ExamNews",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}

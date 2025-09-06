using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace SanjeshP.Mosahebe.Data.Migrations
{
    public partial class Alter_ExamSchedule : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EventTime",
                table: "ExamSchedules");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndTime",
                table: "ExamSchedules",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "StartTime",
                table: "ExamSchedules",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "ExamSchedules");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "ExamSchedules");

            migrationBuilder.AddColumn<DateTime>(
                name: "EventTime",
                table: "ExamSchedules",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}

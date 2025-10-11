using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SanjeshP.Mosahebe.Data.Migrations
{
    public partial class CreateInterviewTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ActiveStation",
                table: "Users",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "QuestionAnswerId",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserFieldId",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "QuestionAnswerId",
                table: "Students",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FieldStationId",
                table: "ExamFields",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserFieldId",
                table: "ExamFields",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FieldStations",
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
                    ExamFieldID = table.Column<int>(nullable: false),
                    StationID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldStations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QuestionAnswers",
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
                    StudentID = table.Column<int>(nullable: false),
                    UserID = table.Column<Guid>(nullable: false),
                    StationID = table.Column<int>(nullable: false),
                    QuestionID = table.Column<int>(nullable: false),
                    Value = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionAnswers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserFields",
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
                    UserID = table.Column<Guid>(nullable: false),
                    ExamFieldID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFields", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Stations",
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
                    Name = table.Column<string>(nullable: true),
                    FieldStationId = table.Column<int>(nullable: true),
                    QuestionAnswerId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Stations_FieldStations_FieldStationId",
                        column: x => x.FieldStationId,
                        principalTable: "FieldStations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Stations_QuestionAnswers_QuestionAnswerId",
                        column: x => x.QuestionAnswerId,
                        principalTable: "QuestionAnswers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StationQuestions",
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
                    Text = table.Column<string>(nullable: true),
                    MaxValue = table.Column<float>(nullable: false),
                    StationId = table.Column<int>(nullable: false),
                    QuestionAnswerId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StationQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StationQuestions_QuestionAnswers_QuestionAnswerId",
                        column: x => x.QuestionAnswerId,
                        principalTable: "QuestionAnswers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StationQuestions_Stations_StationId",
                        column: x => x.StationId,
                        principalTable: "Stations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_QuestionAnswerId",
                table: "Users",
                column: "QuestionAnswerId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserFieldId",
                table: "Users",
                column: "UserFieldId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_QuestionAnswerId",
                table: "Students",
                column: "QuestionAnswerId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamFields_FieldStationId",
                table: "ExamFields",
                column: "FieldStationId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamFields_UserFieldId",
                table: "ExamFields",
                column: "UserFieldId");

            migrationBuilder.CreateIndex(
                name: "IX_StationQuestions_QuestionAnswerId",
                table: "StationQuestions",
                column: "QuestionAnswerId");

            migrationBuilder.CreateIndex(
                name: "IX_StationQuestions_StationId",
                table: "StationQuestions",
                column: "StationId");

            migrationBuilder.CreateIndex(
                name: "IX_Stations_FieldStationId",
                table: "Stations",
                column: "FieldStationId");

            migrationBuilder.CreateIndex(
                name: "IX_Stations_QuestionAnswerId",
                table: "Stations",
                column: "QuestionAnswerId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExamFields_FieldStations_FieldStationId",
                table: "ExamFields",
                column: "FieldStationId",
                principalTable: "FieldStations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ExamFields_UserFields_UserFieldId",
                table: "ExamFields",
                column: "UserFieldId",
                principalTable: "UserFields",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_QuestionAnswers_QuestionAnswerId",
                table: "Students",
                column: "QuestionAnswerId",
                principalTable: "QuestionAnswers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_QuestionAnswers_QuestionAnswerId",
                table: "Users",
                column: "QuestionAnswerId",
                principalTable: "QuestionAnswers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_UserFields_UserFieldId",
                table: "Users",
                column: "UserFieldId",
                principalTable: "UserFields",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExamFields_FieldStations_FieldStationId",
                table: "ExamFields");

            migrationBuilder.DropForeignKey(
                name: "FK_ExamFields_UserFields_UserFieldId",
                table: "ExamFields");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_QuestionAnswers_QuestionAnswerId",
                table: "Students");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_QuestionAnswers_QuestionAnswerId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_UserFields_UserFieldId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "StationQuestions");

            migrationBuilder.DropTable(
                name: "UserFields");

            migrationBuilder.DropTable(
                name: "Stations");

            migrationBuilder.DropTable(
                name: "FieldStations");

            migrationBuilder.DropTable(
                name: "QuestionAnswers");

            migrationBuilder.DropIndex(
                name: "IX_Users_QuestionAnswerId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_UserFieldId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Students_QuestionAnswerId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_ExamFields_FieldStationId",
                table: "ExamFields");

            migrationBuilder.DropIndex(
                name: "IX_ExamFields_UserFieldId",
                table: "ExamFields");

            migrationBuilder.DropColumn(
                name: "ActiveStation",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "QuestionAnswerId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UserFieldId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "QuestionAnswerId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "FieldStationId",
                table: "ExamFields");

            migrationBuilder.DropColumn(
                name: "UserFieldId",
                table: "ExamFields");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SanjeshP.Mosahebe.Data.Migrations
{
    public partial class Add_uploadedFileRepository : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExamFields_FieldStations_FieldStationId",
                table: "ExamFields");

            migrationBuilder.DropForeignKey(
                name: "FK_StationQuestions_QuestionAnswers_QuestionAnswerId",
                table: "StationQuestions");

            migrationBuilder.DropForeignKey(
                name: "FK_Stations_FieldStations_FieldStationId",
                table: "Stations");

            migrationBuilder.DropForeignKey(
                name: "FK_Stations_QuestionAnswers_QuestionAnswerId",
                table: "Stations");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_QuestionAnswers_QuestionAnswerId",
                table: "Students");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_QuestionAnswers_QuestionAnswerId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_QuestionAnswerId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Students_QuestionAnswerId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Stations_FieldStationId",
                table: "Stations");

            migrationBuilder.DropIndex(
                name: "IX_Stations_QuestionAnswerId",
                table: "Stations");

            migrationBuilder.DropIndex(
                name: "IX_StationQuestions_QuestionAnswerId",
                table: "StationQuestions");

            migrationBuilder.DropIndex(
                name: "IX_ExamFields_FieldStationId",
                table: "ExamFields");

            migrationBuilder.DropColumn(
                name: "QuestionAnswerId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "QuestionAnswerId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "FieldStationId",
                table: "Stations");

            migrationBuilder.DropColumn(
                name: "QuestionAnswerId",
                table: "Stations");

            migrationBuilder.DropColumn(
                name: "QuestionAnswerId",
                table: "StationQuestions");

            migrationBuilder.DropColumn(
                name: "FieldStationId",
                table: "ExamFields");

            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "QuestionAnswers",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "StudentID",
                table: "QuestionAnswers",
                newName: "StudentId");

            migrationBuilder.RenameColumn(
                name: "StationID",
                table: "QuestionAnswers",
                newName: "StationId");

            migrationBuilder.RenameColumn(
                name: "QuestionID",
                table: "QuestionAnswers",
                newName: "QuestionId");

            migrationBuilder.RenameColumn(
                name: "StationID",
                table: "FieldStations",
                newName: "StationId");

            migrationBuilder.RenameColumn(
                name: "ExamFieldID",
                table: "FieldStations",
                newName: "ExamFieldId");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "Students",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NationalCode",
                table: "Students",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "Students",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Students",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FatherName",
                table: "Students",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Students",
                maxLength: 250,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CodeRahgiri",
                table: "Students",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Stations",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "StationQuestions",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "MaxValue",
                table: "StationQuestions",
                type: "decimal(6,2)",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<decimal>(
                name: "Value",
                table: "QuestionAnswers",
                type: "decimal(6,2)",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.CreateTable(
                name: "UploadedFiles",
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
                    StudentId = table.Column<int>(nullable: false),
                    StudentFormSubmissionScoreId = table.Column<int>(nullable: true),
                    StoredFileName = table.Column<string>(maxLength: 200, nullable: false),
                    OriginalFileName = table.Column<string>(maxLength: 500, nullable: true),
                    ContentType = table.Column<string>(maxLength: 100, nullable: true),
                    Size = table.Column<long>(nullable: false),
                    UploadedAt = table.Column<DateTime>(nullable: false),
                    RelativePath = table.Column<string>(maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UploadedFiles", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuestionAnswers_QuestionId",
                table: "QuestionAnswers",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionAnswers_StationId",
                table: "QuestionAnswers",
                column: "StationId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionAnswers_StudentId",
                table: "QuestionAnswers",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionAnswers_UserId",
                table: "QuestionAnswers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_FieldStations_ExamFieldId",
                table: "FieldStations",
                column: "ExamFieldId");

            migrationBuilder.CreateIndex(
                name: "IX_FieldStations_StationId",
                table: "FieldStations",
                column: "StationId");

            migrationBuilder.AddForeignKey(
                name: "FK_FieldStations_ExamFields_ExamFieldId",
                table: "FieldStations",
                column: "ExamFieldId",
                principalTable: "ExamFields",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FieldStations_Stations_StationId",
                table: "FieldStations",
                column: "StationId",
                principalTable: "Stations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionAnswers_StationQuestions_QuestionId",
                table: "QuestionAnswers",
                column: "QuestionId",
                principalTable: "StationQuestions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionAnswers_Stations_StationId",
                table: "QuestionAnswers",
                column: "StationId",
                principalTable: "Stations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionAnswers_Students_StudentId",
                table: "QuestionAnswers",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionAnswers_Users_UserId",
                table: "QuestionAnswers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FieldStations_ExamFields_ExamFieldId",
                table: "FieldStations");

            migrationBuilder.DropForeignKey(
                name: "FK_FieldStations_Stations_StationId",
                table: "FieldStations");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionAnswers_StationQuestions_QuestionId",
                table: "QuestionAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionAnswers_Stations_StationId",
                table: "QuestionAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionAnswers_Students_StudentId",
                table: "QuestionAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionAnswers_Users_UserId",
                table: "QuestionAnswers");

            migrationBuilder.DropTable(
                name: "UploadedFiles");

            migrationBuilder.DropIndex(
                name: "IX_QuestionAnswers_QuestionId",
                table: "QuestionAnswers");

            migrationBuilder.DropIndex(
                name: "IX_QuestionAnswers_StationId",
                table: "QuestionAnswers");

            migrationBuilder.DropIndex(
                name: "IX_QuestionAnswers_StudentId",
                table: "QuestionAnswers");

            migrationBuilder.DropIndex(
                name: "IX_QuestionAnswers_UserId",
                table: "QuestionAnswers");

            migrationBuilder.DropIndex(
                name: "IX_FieldStations_ExamFieldId",
                table: "FieldStations");

            migrationBuilder.DropIndex(
                name: "IX_FieldStations_StationId",
                table: "FieldStations");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "QuestionAnswers",
                newName: "UserID");

            migrationBuilder.RenameColumn(
                name: "StudentId",
                table: "QuestionAnswers",
                newName: "StudentID");

            migrationBuilder.RenameColumn(
                name: "StationId",
                table: "QuestionAnswers",
                newName: "StationID");

            migrationBuilder.RenameColumn(
                name: "QuestionId",
                table: "QuestionAnswers",
                newName: "QuestionID");

            migrationBuilder.RenameColumn(
                name: "StationId",
                table: "FieldStations",
                newName: "StationID");

            migrationBuilder.RenameColumn(
                name: "ExamFieldId",
                table: "FieldStations",
                newName: "ExamFieldID");

            migrationBuilder.AddColumn<int>(
                name: "QuestionAnswerId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NationalCode",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "FatherName",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 250,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CodeRahgiri",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "QuestionAnswerId",
                table: "Students",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Stations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 200);

            migrationBuilder.AddColumn<int>(
                name: "FieldStationId",
                table: "Stations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "QuestionAnswerId",
                table: "Stations",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "StationQuestions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<float>(
                name: "MaxValue",
                table: "StationQuestions",
                type: "real",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(6,2)");

            migrationBuilder.AddColumn<int>(
                name: "QuestionAnswerId",
                table: "StationQuestions",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<float>(
                name: "Value",
                table: "QuestionAnswers",
                type: "real",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(6,2)");

            migrationBuilder.AddColumn<int>(
                name: "FieldStationId",
                table: "ExamFields",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_QuestionAnswerId",
                table: "Users",
                column: "QuestionAnswerId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_QuestionAnswerId",
                table: "Students",
                column: "QuestionAnswerId");

            migrationBuilder.CreateIndex(
                name: "IX_Stations_FieldStationId",
                table: "Stations",
                column: "FieldStationId");

            migrationBuilder.CreateIndex(
                name: "IX_Stations_QuestionAnswerId",
                table: "Stations",
                column: "QuestionAnswerId");

            migrationBuilder.CreateIndex(
                name: "IX_StationQuestions_QuestionAnswerId",
                table: "StationQuestions",
                column: "QuestionAnswerId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamFields_FieldStationId",
                table: "ExamFields",
                column: "FieldStationId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExamFields_FieldStations_FieldStationId",
                table: "ExamFields",
                column: "FieldStationId",
                principalTable: "FieldStations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StationQuestions_QuestionAnswers_QuestionAnswerId",
                table: "StationQuestions",
                column: "QuestionAnswerId",
                principalTable: "QuestionAnswers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Stations_FieldStations_FieldStationId",
                table: "Stations",
                column: "FieldStationId",
                principalTable: "FieldStations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Stations_QuestionAnswers_QuestionAnswerId",
                table: "Stations",
                column: "QuestionAnswerId",
                principalTable: "QuestionAnswers",
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
        }
    }
}

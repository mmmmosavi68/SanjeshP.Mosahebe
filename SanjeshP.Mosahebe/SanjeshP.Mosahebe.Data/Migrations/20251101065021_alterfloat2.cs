using Microsoft.EntityFrameworkCore.Migrations;

namespace SanjeshP.Mosahebe.Data.Migrations
{
    public partial class alterfloat2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Score",
                table: "StudentFormSubmissionScores",
                type: "decimal(6,3)",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "MaxValue",
                table: "StationQuestions",
                type: "decimal(6,3)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(8,3)");

            migrationBuilder.AlterColumn<decimal>(
                name: "FinalTotalScore",
                table: "ResearchDocumentScores",
                type: "decimal(6,3)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<decimal>(
                name: "QuestionScore",
                table: "InterviewScores",
                type: "decimal(6,3)",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "QuestionMaxValue",
                table: "InterviewScores",
                type: "decimal(6,3)",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "CalculatedScore",
                table: "InterviewScores",
                type: "decimal(6,3)",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "OneMaxScore",
                table: "FormItems",
                type: "decimal(6,3)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<decimal>(
                name: "AllMaxScore",
                table: "FormItems",
                type: "decimal(6,3)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<decimal>(
                name: "FinalTotalScore",
                table: "EducationDocumentScores",
                type: "decimal(6,3)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<decimal>(
                name: "Value",
                table: "Coefficients",
                type: "decimal(6,3)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Score",
                table: "StudentFormSubmissionScores",
                type: "float",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(6,3)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "MaxValue",
                table: "StationQuestions",
                type: "decimal(8,3)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(6,3)");

            migrationBuilder.AlterColumn<double>(
                name: "FinalTotalScore",
                table: "ResearchDocumentScores",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(6,3)");

            migrationBuilder.AlterColumn<double>(
                name: "QuestionScore",
                table: "InterviewScores",
                type: "float",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(6,3)",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "QuestionMaxValue",
                table: "InterviewScores",
                type: "float",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(6,3)",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "CalculatedScore",
                table: "InterviewScores",
                type: "float",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(6,3)",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "OneMaxScore",
                table: "FormItems",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(6,3)");

            migrationBuilder.AlterColumn<double>(
                name: "AllMaxScore",
                table: "FormItems",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(6,3)");

            migrationBuilder.AlterColumn<double>(
                name: "FinalTotalScore",
                table: "EducationDocumentScores",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(6,3)");

            migrationBuilder.AlterColumn<double>(
                name: "Value",
                table: "Coefficients",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(6,3)");
        }
    }
}

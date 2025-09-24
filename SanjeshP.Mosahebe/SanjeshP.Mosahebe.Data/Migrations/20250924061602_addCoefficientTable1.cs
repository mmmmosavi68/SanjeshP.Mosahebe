using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SanjeshP.Mosahebe.Data.Migrations
{
    public partial class addCoefficientTable1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Coefficients",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CoefficientId = table.Column<int>(nullable: false),
                    CodeFarei = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    RangeStart = table.Column<float>(nullable: false),
                    RangeEnd = table.Column<float>(nullable: false),
                    Value = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coefficients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FormItemCoefficientDefinitions",
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
                    CoefficientId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Label = table.Column<string>(nullable: true),
                    inputType = table.Column<int>(nullable: false),
                    InputTypeText = table.Column<string>(nullable: true),
                    IsRequired = table.Column<bool>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    ValidationPattern = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormItemCoefficientDefinitions", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Coefficients");

            migrationBuilder.DropTable(
                name: "FormItemCoefficientDefinitions");
        }
    }
}

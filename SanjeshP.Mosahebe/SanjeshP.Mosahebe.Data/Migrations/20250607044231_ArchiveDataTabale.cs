using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace SanjeshP.Mosahebe.Data.Migrations
{
    public partial class ArchiveDataTabale : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ArchiveData",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<Guid>(nullable: false),
                    HostIp = table.Column<string>(nullable: true),
                    CodeMeli = table.Column<string>(maxLength: 10, nullable: false),
                    ExamId = table.Column<int>(nullable: false),
                    ExamTitle = table.Column<string>(maxLength: 200, nullable: true),
                    Sal = table.Column<int>(nullable: false),
                    Dore = table.Column<string>(maxLength: 100, nullable: true),
                    JsonContentDescript = table.Column<string>(maxLength: 100, nullable: false),
                    JsonContent = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArchiveData", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArchiveData");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace SanjeshP.Mosahebe.Data.Migrations
{
    public partial class AddLogginSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LoginSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<Guid>(nullable: false),
                    HostIp = table.Column<string>(nullable: true),
                    IsCaptchaEnabled = table.Column<bool>(nullable: false),
                    IsOtpLoginEnabled = table.Column<bool>(nullable: false),
                    IsUsernamePasswordLoginEnabled = table.Column<bool>(nullable: false),
                    IsTwoFactorEnabled = table.Column<bool>(nullable: false),
                    OtpExpirySeconds = table.Column<int>(nullable: false),
                    MaxFailedLoginAttempts = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoginSettings", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LoginSettings");
        }
    }
}

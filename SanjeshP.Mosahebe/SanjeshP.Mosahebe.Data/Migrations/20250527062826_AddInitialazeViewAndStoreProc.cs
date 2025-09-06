using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SanjeshP.Mosahebe.Data.Migrations
{
    public partial class AddInitialazeViewAndStoreProc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var sqlFiles = assembly.GetManifestResourceNames().Where(file => file.EndsWith(".sql"));
            foreach (var sqlFile in sqlFiles)
            {
                using (Stream stream = assembly.GetManifestResourceStream(sqlFile))
                {
                    if (stream == null)
                    {
                        throw new InvalidOperationException($"Embedded resource '{sqlFile}' not found.");
                    }
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        var sqlScript = reader.ReadToEnd();
                        migrationBuilder.Sql($@"
                        USE [SanjeshP_RDC1];
                        {sqlScript}
                    ");
                    }
                }
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var command = "DROP PROCEDURE GetDepartmentSalary";
            migrationBuilder.Sql(command);
        }
    }
}

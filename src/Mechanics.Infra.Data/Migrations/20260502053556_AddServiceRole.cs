using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mechanics.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddServiceRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "Mechanics",
                table: "Roles",
                columns: new[] { "Id", "Name" },
                values: new object[] { new Guid("aaccfd08-66b2-473a-a0a1-2aa7fbbed7cc"), "SERVICE" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "Mechanics",
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("aaccfd08-66b2-473a-a0a1-2aa7fbbed7cc"));
        }
    }
}

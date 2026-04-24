using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Mechanics.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Mechanics");

            migrationBuilder.CreateTable(
                name: "Roles",
                schema: "Mechanics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "Mechanics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CpfNumber = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    SecurityStamp = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "Mechanics",
                        principalTable: "Roles",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                schema: "Mechanics",
                table: "Roles",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("2afde195-550b-498e-a63d-7a6d556b25ba"), "ADMINISTRATOR" },
                    { new Guid("a1097867-aa3e-416c-8685-190516b62a12"), "ATTENDANT" },
                    { new Guid("f31bca41-0895-4af5-976f-ac892f833b1b"), "CUSTOMER_ADMIN" },
                    { new Guid("f6027484-89a4-49f6-a9cb-4d1733c2bab7"), "MECHANIC" },
                    { new Guid("f61b4ae9-cc8f-4fda-a39f-f70bb3c0840f"), "CUSTOMER_USER" }
                });

            migrationBuilder.InsertData(
                schema: "Mechanics",
                table: "Users",
                columns: new[] { "Id", "CpfNumber", "CreationDate", "CustomerId", "Email", "FullName", "PasswordHash", "RoleId", "SecurityStamp" },
                values: new object[,]
                {
                    { new Guid("4c3b8777-6c4a-4bf3-8ad5-aad48981f7f2"), "11144477735", new DateTime(2025, 10, 12, 12, 0, 0, 0, DateTimeKind.Utc), null, "mechanic@mechanics.com", "Mechanic User", "AQAAAAIAAYagAAAAEKSeHdHtCfN38pakeil4oyEL0d07GBEySe6csY8jmXIKT3oEZVcZR7Jngd9qxFgmkQ==", new Guid("f6027484-89a4-49f6-a9cb-4d1733c2bab7"), "0a3bc211-1220-4d20-80e9-bd850d0dc200" },
                    { new Guid("c2a83e5a-27c7-440a-97e3-86234eebb3c7"), "98765432100", new DateTime(2025, 10, 12, 12, 0, 0, 0, DateTimeKind.Utc), null, "attendant@mechanics.com", "Attendant User", "AQAAAAIAAYagAAAAEEo/VptbCYVPiVkoEVHthpWAZUvV/KJ0WJkg+wKbtXJkmMHmSfnpFT4JTLofugBwyQ==", new Guid("a1097867-aa3e-416c-8685-190516b62a12"), "370c4d16-8e11-46ca-9004-e1fb9311e49e" },
                    { new Guid("db27b85d-b0f3-4300-bb45-7841f0d11617"), "12345678909", new DateTime(2025, 10, 12, 12, 0, 0, 0, DateTimeKind.Utc), null, "administrator@mechanics.com", "Administrator User", "AQAAAAIAAYagAAAAEPGF9Xsz+ARiCopDgQbQ8gbGubN6bhvNhpKiy8XK2BORE5eV95VywrM9rVE48i2m8w==", new Guid("2afde195-550b-498e-a63d-7a6d556b25ba"), "efcaaf76-0535-45fc-a79c-06ab92c064bb" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Name",
                schema: "Mechanics",
                table: "Roles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_CpfNumber",
                schema: "Mechanics",
                table: "Users",
                column: "CpfNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                schema: "Mechanics",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_FullName",
                schema: "Mechanics",
                table: "Users",
                column: "FullName");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                schema: "Mechanics",
                table: "Users",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users",
                schema: "Mechanics");

            migrationBuilder.DropTable(
                name: "Roles",
                schema: "Mechanics");
        }
    }
}

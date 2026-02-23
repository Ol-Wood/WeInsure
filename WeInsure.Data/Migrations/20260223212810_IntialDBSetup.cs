using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WeInsure.Data.Migrations
{
    /// <inheritdoc />
    public partial class IntialDBSetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Policies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    PolicyType = table.Column<int>(type: "INTEGER", nullable: false),
                    Price = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    Reference = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    AutoRenew = table.Column<bool>(type: "INTEGER", nullable: false),
                    Canceled = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Policies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InsuredProperties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    PolicyId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Address_AddressLine1 = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Address_AddressLine2 = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Address_AddressLine3 = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Address_PostCode = table.Column<string>(type: "TEXT", maxLength: 8, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsuredProperties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InsuredProperties_Policies_PolicyId",
                        column: x => x.PolicyId,
                        principalTable: "Policies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    PolicyId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PaymentType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Amount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    Reference = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_Policies_PolicyId",
                        column: x => x.PolicyId,
                        principalTable: "Policies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PolicyHolders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    PolicyId = table.Column<Guid>(type: "TEXT", nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    PolicyId1 = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PolicyHolders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PolicyHolders_Policies_PolicyId",
                        column: x => x.PolicyId,
                        principalTable: "Policies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PolicyHolders_Policies_PolicyId1",
                        column: x => x.PolicyId1,
                        principalTable: "Policies",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_InsuredProperties_PolicyId",
                table: "InsuredProperties",
                column: "PolicyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_PolicyId",
                table: "Payments",
                column: "PolicyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PolicyHolders_PolicyId",
                table: "PolicyHolders",
                column: "PolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_PolicyHolders_PolicyId1",
                table: "PolicyHolders",
                column: "PolicyId1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InsuredProperties");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "PolicyHolders");

            migrationBuilder.DropTable(
                name: "Policies");
        }
    }
}

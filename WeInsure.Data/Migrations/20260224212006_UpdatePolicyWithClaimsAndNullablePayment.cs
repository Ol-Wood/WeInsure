using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WeInsure.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePolicyWithClaimsAndNullablePayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasClaims",
                table: "Policies",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasClaims",
                table: "Policies");
        }
    }
}

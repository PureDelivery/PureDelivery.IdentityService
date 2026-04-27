using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PureDelivery.IdentityService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DecimalToDouble : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Must drop CHECK constraints and index before altering column type
            migrationBuilder.DropCheckConstraint(
                name: "CK_CustomerAddress_Coordinates_Latitude",
                table: "CustomerAddresses");

            migrationBuilder.DropCheckConstraint(
                name: "CK_CustomerAddress_Coordinates_Longitude",
                table: "CustomerAddresses");

            migrationBuilder.DropIndex(
                name: "CustomerAddresses_Location",
                table: "CustomerAddresses");

            migrationBuilder.AlterColumn<double>(
                name: "Longitude",
                table: "CustomerAddresses",
                type: "float",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(11,8)",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Latitude",
                table: "CustomerAddresses",
                type: "float",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,8)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "CustomerAddresses_Location",
                table: "CustomerAddresses",
                columns: new[] { "Latitude", "Longitude" });

            migrationBuilder.AddCheckConstraint(
                name: "CK_CustomerAddress_Coordinates_Latitude",
                table: "CustomerAddresses",
                sql: "[Latitude] IS NULL OR ([Latitude] >= -90 AND [Latitude] <= 90)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_CustomerAddress_Coordinates_Longitude",
                table: "CustomerAddresses",
                sql: "[Longitude] IS NULL OR ([Longitude] >= -180 AND [Longitude] <= 180)");

            migrationBuilder.AlterColumn<decimal>(
                name: "LoyaltyPoints",
                table: "CustomerProfiles",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldDefaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_CustomerAddress_Coordinates_Latitude",
                table: "CustomerAddresses");

            migrationBuilder.DropCheckConstraint(
                name: "CK_CustomerAddress_Coordinates_Longitude",
                table: "CustomerAddresses");

            migrationBuilder.DropIndex(
                name: "CustomerAddresses_Location",
                table: "CustomerAddresses");

            migrationBuilder.AlterColumn<decimal>(
                name: "Longitude",
                table: "CustomerAddresses",
                type: "decimal(11,8)",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Latitude",
                table: "CustomerAddresses",
                type: "decimal(10,8)",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "CustomerAddresses_Location",
                table: "CustomerAddresses",
                columns: new[] { "Latitude", "Longitude" });

            migrationBuilder.AddCheckConstraint(
                name: "CK_CustomerAddress_Coordinates_Latitude",
                table: "CustomerAddresses",
                sql: "[Latitude] IS NULL OR ([Latitude] >= -90 AND [Latitude] <= 90)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_CustomerAddress_Coordinates_Longitude",
                table: "CustomerAddresses",
                sql: "[Longitude] IS NULL OR ([Longitude] >= -180 AND [Longitude] <= 180)");

            migrationBuilder.AlterColumn<decimal>(
                name: "LoyaltyPoints",
                table: "CustomerProfiles",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)",
                oldPrecision: 18,
                oldScale: 4,
                oldDefaultValue: 0m);
        }
    }
}

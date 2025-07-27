using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PureDelivery.IdentityService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class OtpCodeProps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EmailConfirmationAttempts",
                table: "Customers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "EmailConfirmationOtp",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EmailConfirmationOtpExpiry",
                table: "Customers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsEmailConfirmed",
                table: "Customers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastOtpSentAt",
                table: "Customers",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailConfirmationAttempts",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "EmailConfirmationOtp",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "EmailConfirmationOtpExpiry",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "IsEmailConfirmed",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "LastOtpSentAt",
                table: "Customers");
        }
    }
}

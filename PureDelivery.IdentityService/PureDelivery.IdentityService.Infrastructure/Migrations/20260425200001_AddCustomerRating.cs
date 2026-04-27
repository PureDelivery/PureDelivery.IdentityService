using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PureDelivery.IdentityService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerRating : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomerRating",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourierId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CustomerProfileCustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerRating", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerRating_CustomerProfiles_CustomerProfileCustomerId",
                        column: x => x.CustomerProfileCustomerId,
                        principalTable: "CustomerProfiles",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerRating_CustomerProfileCustomerId",
                table: "CustomerRating",
                column: "CustomerProfileCustomerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerRating");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PureDelivery.IdentityService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAvatarUrlToCustomerProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AvatarUrl",
                table: "CustomerProfiles",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvatarUrl",
                table: "CustomerProfiles");
        }
    }
}

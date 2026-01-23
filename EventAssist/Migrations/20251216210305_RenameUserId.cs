using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventAssist.Migrations
{
    /// <inheritdoc />
    public partial class RenameUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Events");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Events",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}

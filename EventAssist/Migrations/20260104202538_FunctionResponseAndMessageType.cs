using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventAssist.Migrations
{
    /// <inheritdoc />
    public partial class FunctionResponseAndMessageType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000);

            migrationBuilder.AddColumn<string>(
                name: "FunctionResponse",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Messages",
                type: "int",
                maxLength: 2000,
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FunctionResponse",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Messages");

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "Messages",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}

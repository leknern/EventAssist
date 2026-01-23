using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventAssist.Migrations
{
    /// <inheritdoc />
    public partial class helpdeskAgentAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chats_Users_UserId",
                table: "Chats");

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "Messages",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Events",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Events",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "HelpdeskAgentId",
                table: "Chats",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserRecordId",
                table: "Chats",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Chats_HelpdeskAgentId",
                table: "Chats",
                column: "HelpdeskAgentId");

            migrationBuilder.CreateIndex(
                name: "IX_Chats_UserRecordId",
                table: "Chats",
                column: "UserRecordId");

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_Users_HelpdeskAgentId",
                table: "Chats",
                column: "HelpdeskAgentId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_Users_UserId",
                table: "Chats",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_Users_UserRecordId",
                table: "Chats",
                column: "UserRecordId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chats_Users_HelpdeskAgentId",
                table: "Chats");

            migrationBuilder.DropForeignKey(
                name: "FK_Chats_Users_UserId",
                table: "Chats");

            migrationBuilder.DropForeignKey(
                name: "FK_Chats_Users_UserRecordId",
                table: "Chats");

            migrationBuilder.DropIndex(
                name: "IX_Chats_HelpdeskAgentId",
                table: "Chats");

            migrationBuilder.DropIndex(
                name: "IX_Chats_UserRecordId",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "HelpdeskAgentId",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "UserRecordId",
                table: "Chats");

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Events",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Events",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_Users_UserId",
                table: "Chats",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

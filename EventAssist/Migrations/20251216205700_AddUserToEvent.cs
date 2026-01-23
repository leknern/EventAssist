using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventAssist.Migrations
{
    /// <inheritdoc />
    public partial class AddUserToEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Users_UserRecordId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Chats_ChatRecordId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_ChatRecordId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "ChatRecordId",
                table: "Messages");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Events",
                newName: "Name");

            migrationBuilder.AddColumn<int>(
                name: "ChatId",
                table: "Messages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "UserRecordId",
                table: "Events",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Events",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Events",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ChatId",
                table: "Messages",
                column: "ChatId");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Users_UserRecordId",
                table: "Events",
                column: "UserRecordId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Chats_ChatId",
                table: "Messages",
                column: "ChatId",
                principalTable: "Chats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Users_UserRecordId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Chats_ChatId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_ChatId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "ChatId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Events");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Events",
                newName: "Title");

            migrationBuilder.AddColumn<int>(
                name: "ChatRecordId",
                table: "Messages",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UserRecordId",
                table: "Events",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Events",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ChatRecordId",
                table: "Messages",
                column: "ChatRecordId");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Users_UserRecordId",
                table: "Events",
                column: "UserRecordId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Chats_ChatRecordId",
                table: "Messages",
                column: "ChatRecordId",
                principalTable: "Chats",
                principalColumn: "Id");
        }
    }
}

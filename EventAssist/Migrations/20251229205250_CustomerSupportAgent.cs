using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventAssist.Migrations
{
    /// <inheritdoc />
    public partial class CustomerSupportAgent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chats_Users_CustomerSupportOperatorId",
                table: "Chats");

            migrationBuilder.RenameColumn(
                name: "CustomerSupportOperatorId",
                table: "Chats",
                newName: "CustomerSupportAgentId");

            migrationBuilder.RenameColumn(
                name: "CustomerSupportComment",
                table: "Chats",
                newName: "InternalNote");

            migrationBuilder.RenameIndex(
                name: "IX_Chats_CustomerSupportOperatorId",
                table: "Chats",
                newName: "IX_Chats_CustomerSupportAgentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_Users_CustomerSupportAgentId",
                table: "Chats",
                column: "CustomerSupportAgentId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chats_Users_CustomerSupportAgentId",
                table: "Chats");

            migrationBuilder.RenameColumn(
                name: "InternalNote",
                table: "Chats",
                newName: "CustomerSupportComment");

            migrationBuilder.RenameColumn(
                name: "CustomerSupportAgentId",
                table: "Chats",
                newName: "CustomerSupportOperatorId");

            migrationBuilder.RenameIndex(
                name: "IX_Chats_CustomerSupportAgentId",
                table: "Chats",
                newName: "IX_Chats_CustomerSupportOperatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_Users_CustomerSupportOperatorId",
                table: "Chats",
                column: "CustomerSupportOperatorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

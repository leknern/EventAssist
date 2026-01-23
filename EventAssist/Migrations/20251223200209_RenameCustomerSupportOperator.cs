using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventAssist.Migrations
{
    /// <inheritdoc />
    public partial class RenameCustomerSupportOperator : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chats_Users_HelpdeskAgentId",
                table: "Chats");

            migrationBuilder.RenameColumn(
                name: "HelpdeskAgentId",
                table: "Chats",
                newName: "CustomerSupportOperatorId");

            migrationBuilder.RenameIndex(
                name: "IX_Chats_HelpdeskAgentId",
                table: "Chats",
                newName: "IX_Chats_CustomerSupportOperatorId");

            migrationBuilder.AddColumn<bool>(
                name: "HasCustomerSupportOperator",
                table: "Chats",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_Users_CustomerSupportOperatorId",
                table: "Chats",
                column: "CustomerSupportOperatorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chats_Users_CustomerSupportOperatorId",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "HasCustomerSupportOperator",
                table: "Chats");

            migrationBuilder.RenameColumn(
                name: "CustomerSupportOperatorId",
                table: "Chats",
                newName: "HelpdeskAgentId");

            migrationBuilder.RenameIndex(
                name: "IX_Chats_CustomerSupportOperatorId",
                table: "Chats",
                newName: "IX_Chats_HelpdeskAgentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_Users_HelpdeskAgentId",
                table: "Chats",
                column: "HelpdeskAgentId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

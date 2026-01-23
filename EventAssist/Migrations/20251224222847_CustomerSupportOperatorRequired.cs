using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventAssist.Migrations
{
    /// <inheritdoc />
    public partial class CustomerSupportOperatorRequired : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "HasCustomerSupportOperator",
                table: "Chats",
                newName: "CustomerSupportOperatorRequired");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CustomerSupportOperatorRequired",
                table: "Chats",
                newName: "HasCustomerSupportOperator");
        }
    }
}

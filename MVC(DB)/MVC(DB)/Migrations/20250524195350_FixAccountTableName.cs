using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVC_DB_.Migrations
{
    public partial class FixAccountTableName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_registerInformation",
                table: "registerInformation");

            migrationBuilder.RenameTable(
                name: "registerInformation",
                newName: "accountInformation");

            migrationBuilder.AddPrimaryKey(
                name: "PK_accountInformation",
                table: "accountInformation",
                column: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_accountInformation",
                table: "accountInformation");

            migrationBuilder.RenameTable(
                name: "accountInformation",
                newName: "registerInformation");

            migrationBuilder.AddPrimaryKey(
                name: "PK_registerInformation",
                table: "registerInformation",
                column: "id");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVC_DB_.Migrations
{
    public partial class UpdateCampaignModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Campaigns");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Campaigns",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobPortalMud.Server.Migrations
{
    public partial class addStoredFileNameEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StoredFileName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StoredFileName",
                table: "AspNetUsers");
        }
    }
}

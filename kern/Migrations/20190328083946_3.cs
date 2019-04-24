using Microsoft.EntityFrameworkCore.Migrations;

namespace kern.Migrations
{
    public partial class _3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastEntry",
                table: "AccauntUsers",
                newName: "CreateDate");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "AccauntUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "AccauntUsers");

            migrationBuilder.RenameColumn(
                name: "CreateDate",
                table: "AccauntUsers",
                newName: "LastEntry");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace kern.Migrations
{
    public partial class _5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "isValid",
                table: "AccauntUsers",
                newName: "IsValid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsValid",
                table: "AccauntUsers",
                newName: "isValid");
        }
    }
}

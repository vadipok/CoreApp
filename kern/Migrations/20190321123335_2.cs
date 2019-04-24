using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace kern.Migrations
{
    public partial class _2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccauntRoles",
                columns: table => new
                {
                    IdRole = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    RoleName = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    isValid = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccauntRoles", x => x.IdRole);
                });

            migrationBuilder.CreateTable(
                name: "AccauntUsersRoles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    FK_AccauntUser = table.Column<int>(nullable: false),
                    AccauntUser = table.Column<int>(nullable: true),
                    IdRole = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccauntUsersRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccauntUsersRoles_AccauntUsers_AccauntUser",
                        column: x => x.AccauntUser,
                        principalTable: "AccauntUsers",
                        principalColumn: "IdUser",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccauntUsersRoles_AccauntUser",
                table: "AccauntUsersRoles",
                column: "AccauntUser");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccauntRoles");

            migrationBuilder.DropTable(
                name: "AccauntUsersRoles");
        }
    }
}

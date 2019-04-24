using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace kern.Migrations
{
    public partial class _4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FK_AccauntUser",
                table: "AccauntUsersRoles");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                table: "AccauntUsersRoles",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreateDate",
                table: "AccauntUsersRoles");

            migrationBuilder.AddColumn<int>(
                name: "FK_AccauntUser",
                table: "AccauntUsersRoles",
                nullable: false,
                defaultValue: 0);
        }
    }
}

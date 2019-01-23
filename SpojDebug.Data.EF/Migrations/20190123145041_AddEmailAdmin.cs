using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SpojDebug.Data.EF.Migrations
{
    public partial class AddEmailAdmin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SystemEmail",
                table: "AdminSetting",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SystemEmailPasswordEncode",
                table: "AdminSetting",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SystemEmail",
                table: "AdminSetting");

            migrationBuilder.DropColumn(
                name: "SystemEmailPasswordEncode",
                table: "AdminSetting");
        }
    }
}

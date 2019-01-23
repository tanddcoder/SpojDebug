using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SpojDebug.Data.EF.Migrations
{
    public partial class TestCaseLimitation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContestName",
                table: "AdminSetting",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TestCaseLimit",
                table: "AdminSetting",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContestName",
                table: "AdminSetting");

            migrationBuilder.DropColumn(
                name: "TestCaseLimit",
                table: "AdminSetting");
        }
    }
}

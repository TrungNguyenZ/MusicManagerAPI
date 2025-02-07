using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicManager.Migrations
{
    public partial class addRefreshToken : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "quarter",
                table: "DataModels",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "quarterYear",
                table: "DataModels",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "CountryPercentModel",
                columns: table => new
                {
                    quarter = table.Column<int>(type: "int", nullable: false),
                    quarterYear = table.Column<int>(type: "int", nullable: false),
                    countryCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    sum = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "DigitalMonthSumModel",
                columns: table => new
                {
                    month = table.Column<int>(type: "int", nullable: false),
                    year = table.Column<int>(type: "int", nullable: false),
                    digitalServiceProvider = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    sum = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "DigitalQuarterPercentModel",
                columns: table => new
                {
                    quarter = table.Column<int>(type: "int", nullable: false),
                    quarterYear = table.Column<int>(type: "int", nullable: false),
                    digitalServiceProvider = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    sum = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "DigitalQuarterSumModel",
                columns: table => new
                {
                    quarter = table.Column<int>(type: "int", nullable: false),
                    year = table.Column<int>(type: "int", nullable: false),
                    digitalServiceProvider = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    sum = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "DigitalYearPercentModel",
                columns: table => new
                {
                    quarter = table.Column<int>(type: "int", nullable: false),
                    quarterYear = table.Column<int>(type: "int", nullable: false),
                    digitalServiceProvider = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    sum = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "DigitalYearSumModel",
                columns: table => new
                {
                    quarterYear = table.Column<int>(type: "int", nullable: false),
                    digitalServiceProvider = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    sum = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StatisticPriceNameModel",
                columns: table => new
                {
                    priceName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    sum = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "StatisticYoutubeModel",
                columns: table => new
                {
                    revenue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    sum = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "TopChartArtist",
                columns: table => new
                {
                    artistName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    sum = table.Column<long>(type: "bigint", nullable: false),
                    percentage = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "TopChartTrack",
                columns: table => new
                {
                    catalogueTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    artistName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    sum = table.Column<long>(type: "bigint", nullable: false),
                    percentage = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CountryPercentModel");

            migrationBuilder.DropTable(
                name: "DigitalMonthSumModel");

            migrationBuilder.DropTable(
                name: "DigitalQuarterPercentModel");

            migrationBuilder.DropTable(
                name: "DigitalQuarterSumModel");

            migrationBuilder.DropTable(
                name: "DigitalYearPercentModel");

            migrationBuilder.DropTable(
                name: "DigitalYearSumModel");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "StatisticPriceNameModel");

            migrationBuilder.DropTable(
                name: "StatisticYoutubeModel");

            migrationBuilder.DropTable(
                name: "TopChartArtist");

            migrationBuilder.DropTable(
                name: "TopChartTrack");

            migrationBuilder.DropColumn(
                name: "quarter",
                table: "DataModels");

            migrationBuilder.DropColumn(
                name: "quarterYear",
                table: "DataModels");

            migrationBuilder.DropColumn(
                name: "Active",
                table: "AspNetUsers");
        }
    }
}

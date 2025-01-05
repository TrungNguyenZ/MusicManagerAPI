using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicManager.Migrations
{
    public partial class AddFieldUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ArtistName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "RevenuePercentage",
                table: "AspNetUsers",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "StatisticTotalModels",
                columns: table => new
                {
                    TotalForQuarterYear = table.Column<long>(type: "bigint", nullable: false),
                    TotalForYear = table.Column<long>(type: "bigint", nullable: false),
                    TotalForAll = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StatisticTotalModels");

            migrationBuilder.DropColumn(
                name: "ArtistName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "RevenuePercentage",
                table: "AspNetUsers");
        }
    }
}

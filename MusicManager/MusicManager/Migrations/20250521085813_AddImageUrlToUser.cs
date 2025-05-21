using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicManager.Migrations
{
    public partial class AddImageUrlToUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RefreshTokens",
                table: "RefreshTokens");

            migrationBuilder.RenameTable(
                name: "RefreshTokens",
                newName: "RefreshToken");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsEnterprise",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_RefreshToken",
                table: "RefreshToken",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "DataExportExcelModel",
                columns: table => new
                {
                    marketingOwner = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    artistName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    projectTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    catalogueNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    isrc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    catalogueTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    reportedMon = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    digitalServiceProvider = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    countryCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    countryDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    priceName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    revenueTypeDesc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    sale = table.Column<int>(type: "int", nullable: false),
                    netIncome = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "TableRevenue",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    isEnterprise = table.Column<bool>(type: "bit", nullable: false),
                    RevenuePercentage = table.Column<double>(type: "float", nullable: false),
                    TotalNetIncome = table.Column<long>(type: "bigint", nullable: false),
                    UserRevenue = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DataExportExcelModel");

            migrationBuilder.DropTable(
                name: "TableRevenue");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RefreshToken",
                table: "RefreshToken");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsEnterprise",
                table: "AspNetUsers");

            migrationBuilder.RenameTable(
                name: "RefreshToken",
                newName: "RefreshTokens");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RefreshTokens",
                table: "RefreshTokens",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });
        }
    }
}

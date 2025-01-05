using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicManager.Migrations
{
    public partial class AddDataTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DataModels",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
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
                    exchRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    grossIncome = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    distributionFees = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    netIncome = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    netIncomeCompany = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    netIncomeSinger = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    month = table.Column<int>(type: "int", nullable: false),
                    year = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataModels", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DataModels");
        }
    }
}

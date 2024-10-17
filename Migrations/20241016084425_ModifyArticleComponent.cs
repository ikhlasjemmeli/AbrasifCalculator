using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Calculator.Migrations
{
    public partial class ModifyArticleComponent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "numberOfPieces",
                table: "ArticleComponents",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "totalpriceHT",
                table: "ArticleComponents",
                type: "decimal(16,3)",
                precision: 16,
                scale: 3,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "totalpriceTTC",
                table: "ArticleComponents",
                type: "decimal(16,3)",
                precision: 16,
                scale: 3,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "tva",
                table: "ArticleComponents",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "numberOfPieces",
                table: "ArticleComponents");

            migrationBuilder.DropColumn(
                name: "totalpriceHT",
                table: "ArticleComponents");

            migrationBuilder.DropColumn(
                name: "totalpriceTTC",
                table: "ArticleComponents");

            migrationBuilder.DropColumn(
                name: "tva",
                table: "ArticleComponents");
        }
    }
}

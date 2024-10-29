using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Calculator.Migrations
{
    public partial class addNumberOfPiecesToArticle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "numberOfPieces",
                table: "ArticleComponents");

            migrationBuilder.AddColumn<int>(
                name: "numberOfPieces",
                table: "Articles",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "numberOfPieces",
                table: "Articles");

            migrationBuilder.AddColumn<int>(
                name: "numberOfPieces",
                table: "ArticleComponents",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}

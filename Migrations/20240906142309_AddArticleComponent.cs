using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Calculator.Migrations
{
    public partial class AddArticleComponent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "Components");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Components");

            migrationBuilder.CreateTable(
                name: "ArticleComponents",
                columns: table => new
                {
                    ArticleId = table.Column<int>(type: "int", nullable: false),
                    ComponentId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(16,3)", precision: 16, scale: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleComponents", x => new { x.ArticleId, x.ComponentId });
                    table.ForeignKey(
                        name: "FK_ArticleComponents_Articles_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Articles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArticleComponents_Components_ComponentId",
                        column: x => x.ComponentId,
                        principalTable: "Components",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArticleComponents_ComponentId",
                table: "ArticleComponents",
                column: "ComponentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArticleComponents");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Components",
                type: "decimal(16,3)",
                precision: 16,
                scale: 3,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "Components",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}

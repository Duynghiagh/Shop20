using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShoppingDemo.Migrations
{
    /// <inheritdoc />
    public partial class rating1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ratingModels_ProductId",
                table: "ratingModels");

            migrationBuilder.RenameColumn(
                name: "Rating",
                table: "ratingModels",
                newName: "Star");

            migrationBuilder.CreateIndex(
                name: "IX_ratingModels_ProductId",
                table: "ratingModels",
                column: "ProductId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ratingModels_ProductId",
                table: "ratingModels");

            migrationBuilder.RenameColumn(
                name: "Star",
                table: "ratingModels",
                newName: "Rating");

            migrationBuilder.CreateIndex(
                name: "IX_ratingModels_ProductId",
                table: "ratingModels",
                column: "ProductId");
        }
    }
}

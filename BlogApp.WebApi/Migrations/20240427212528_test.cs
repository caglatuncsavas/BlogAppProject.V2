using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogApp.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class test : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogPostCategory_Categories_CatgeoriesId",
                table: "BlogPostCategory");

            migrationBuilder.RenameColumn(
                name: "CatgeoriesId",
                table: "BlogPostCategory",
                newName: "CategoriesId");

            migrationBuilder.RenameIndex(
                name: "IX_BlogPostCategory_CatgeoriesId",
                table: "BlogPostCategory",
                newName: "IX_BlogPostCategory_CategoriesId");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogPostCategory_Categories_CategoriesId",
                table: "BlogPostCategory",
                column: "CategoriesId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogPostCategory_Categories_CategoriesId",
                table: "BlogPostCategory");

            migrationBuilder.RenameColumn(
                name: "CategoriesId",
                table: "BlogPostCategory",
                newName: "CatgeoriesId");

            migrationBuilder.RenameIndex(
                name: "IX_BlogPostCategory_CategoriesId",
                table: "BlogPostCategory",
                newName: "IX_BlogPostCategory_CatgeoriesId");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogPostCategory_Categories_CatgeoriesId",
                table: "BlogPostCategory",
                column: "CatgeoriesId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

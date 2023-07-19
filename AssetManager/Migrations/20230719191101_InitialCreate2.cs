using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssetManager.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assets_AspNetUsers_UserId",
                table: "Assets");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetsCategories_AspNetUsers_UserId",
                table: "AssetsCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetsCategories_Assets_AssetId",
                table: "AssetsCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetsCategories_Categories_CategoryId",
                table: "AssetsCategories");

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_AspNetUsers_UserId",
                table: "Assets",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetsCategories_AspNetUsers_UserId",
                table: "AssetsCategories",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetsCategories_Assets_AssetId",
                table: "AssetsCategories",
                column: "AssetId",
                principalTable: "Assets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetsCategories_Categories_CategoryId",
                table: "AssetsCategories",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assets_AspNetUsers_UserId",
                table: "Assets");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetsCategories_AspNetUsers_UserId",
                table: "AssetsCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetsCategories_Assets_AssetId",
                table: "AssetsCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetsCategories_Categories_CategoryId",
                table: "AssetsCategories");

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_AspNetUsers_UserId",
                table: "Assets",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetsCategories_AspNetUsers_UserId",
                table: "AssetsCategories",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetsCategories_Assets_AssetId",
                table: "AssetsCategories",
                column: "AssetId",
                principalTable: "Assets",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetsCategories_Categories_CategoryId",
                table: "AssetsCategories",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

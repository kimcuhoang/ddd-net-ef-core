using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DDDEfCore.ProductCatalog.Infrastructure.EfCore.Migrations
{
    public partial class Database_Initialize : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Catalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DisplayName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Catalog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DisplayName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CatalogCategory",
                columns: table => new
                {
                    CatalogCategoryId = table.Column<Guid>(nullable: false),
                    CatalogId = table.Column<Guid>(nullable: false),
                    CategoryId = table.Column<Guid>(nullable: false),
                    DisplayName = table.Column<string>(nullable: true),
                    CatalogCategoryParentId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogCategory", x => x.CatalogCategoryId);
                    table.ForeignKey(
                        name: "FK_CatalogCategory_CatalogCategory_CatalogCategoryParentId",
                        column: x => x.CatalogCategoryParentId,
                        principalTable: "CatalogCategory",
                        principalColumn: "CatalogCategoryId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CatalogCategory_Catalog_CatalogId",
                        column: x => x.CatalogId,
                        principalTable: "Catalog",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CatalogProduct",
                columns: table => new
                {
                    CatalogProductId = table.Column<Guid>(nullable: false),
                    DisplayName = table.Column<string>(nullable: true),
                    ProductId = table.Column<Guid>(nullable: false),
                    CatalogCategoryId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogProduct", x => x.CatalogProductId);
                    table.ForeignKey(
                        name: "FK_CatalogProduct_CatalogCategory_CatalogCategoryId",
                        column: x => x.CatalogCategoryId,
                        principalTable: "CatalogCategory",
                        principalColumn: "CatalogCategoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CatalogCategory_CatalogCategoryParentId",
                table: "CatalogCategory",
                column: "CatalogCategoryParentId");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogCategory_CatalogId",
                table: "CatalogCategory",
                column: "CatalogId");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogProduct_CatalogCategoryId",
                table: "CatalogProduct",
                column: "CatalogCategoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CatalogProduct");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "Product");

            migrationBuilder.DropTable(
                name: "CatalogCategory");

            migrationBuilder.DropTable(
                name: "Catalog");
        }
    }
}

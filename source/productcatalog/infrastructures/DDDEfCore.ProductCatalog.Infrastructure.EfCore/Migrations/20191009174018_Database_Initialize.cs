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
                name: "CatalogCategory",
                columns: table => new
                {
                    CatalogCategoryId = table.Column<Guid>(nullable: false),
                    CatalogId = table.Column<Guid>(nullable: false),
                    CategoryId = table.Column<Guid>(nullable: false),
                    DisplayName = table.Column<string>(nullable: true),
                    AvailableFromDate = table.Column<DateTime>(nullable: false),
                    AvailableToDate = table.Column<DateTime>(nullable: true),
                    ParentId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogCategory", x => x.CatalogCategoryId);
                    table.ForeignKey(
                        name: "FK_CatalogCategory_Catalog_CatalogId",
                        column: x => x.CatalogId,
                        principalTable: "Catalog",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CatalogCategory_CatalogCategory_ParentId",
                        column: x => x.ParentId,
                        principalTable: "CatalogCategory",
                        principalColumn: "CatalogCategoryId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CatalogCategory_CatalogId",
                table: "CatalogCategory",
                column: "CatalogId");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogCategory_ParentId",
                table: "CatalogCategory",
                column: "ParentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CatalogCategory");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "Catalog");
        }
    }
}

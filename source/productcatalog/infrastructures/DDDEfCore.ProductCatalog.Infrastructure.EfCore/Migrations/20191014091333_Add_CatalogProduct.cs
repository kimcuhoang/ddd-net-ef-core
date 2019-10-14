using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DDDEfCore.ProductCatalog.Infrastructure.EfCore.Migrations
{
    public partial class Add_CatalogProduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CatalogProduct",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DisplayName = table.Column<string>(nullable: true),
                    ProductId = table.Column<Guid>(nullable: false),
                    CatalogCategoryId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogProduct", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CatalogProduct_CatalogCategory_CatalogCategoryId",
                        column: x => x.CatalogCategoryId,
                        principalTable: "CatalogCategory",
                        principalColumn: "CatalogCategoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CatalogProduct_CatalogCategoryId",
                table: "CatalogProduct",
                column: "CatalogCategoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CatalogProduct");
        }
    }
}

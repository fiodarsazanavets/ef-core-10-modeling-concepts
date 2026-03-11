using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddExplicitRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderLines_Products_ProductId",
                schema: "shop",
                table: "OrderLines");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Customers_CustomerId",
                schema: "shop",
                table: "Orders");

            migrationBuilder.AddColumn<int>(
                name: "SalesAgentId",
                schema: "shop",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProductId1",
                schema: "shop",
                table: "OrderLines",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CustomerProfiles",
                schema: "shop",
                columns: table => new
                {
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    LoyaltyTier = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateOfBirthUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerProfiles", x => x.CustomerId);
                    table.ForeignKey(
                        name: "FK_CustomerProfiles_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalSchema: "shop",
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SalesAgents",
                schema: "shop",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesAgents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Suppliers",
                schema: "shop",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suppliers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                schema: "shop",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductSuppliers",
                schema: "shop",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    SupplierId = table.Column<int>(type: "int", nullable: false),
                    ContractPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    LeadTimeDays = table.Column<int>(type: "int", nullable: false),
                    IsPreferred = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductSuppliers", x => new { x.ProductId, x.SupplierId });
                    table.ForeignKey(
                        name: "FK_ProductSuppliers_Products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "shop",
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductSuppliers_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalSchema: "shop",
                        principalTable: "Suppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductTags",
                schema: "shop",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    TagId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductTags", x => new { x.ProductId, x.TagId });
                    table.ForeignKey(
                        name: "FK_ProductTags_Products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "shop",
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductTags_Tags_TagId",
                        column: x => x.TagId,
                        principalSchema: "shop",
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_SalesAgentId",
                schema: "shop",
                table: "Orders",
                column: "SalesAgentId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderLines_ProductId1",
                schema: "shop",
                table: "OrderLines",
                column: "ProductId1");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSuppliers_SupplierId",
                schema: "shop",
                table: "ProductSuppliers",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductTags_TagId",
                schema: "shop",
                table: "ProductTags",
                column: "TagId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderLines_Products_ProductId",
                schema: "shop",
                table: "OrderLines",
                column: "ProductId",
                principalSchema: "shop",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderLines_Products_ProductId1",
                schema: "shop",
                table: "OrderLines",
                column: "ProductId1",
                principalSchema: "shop",
                principalTable: "Products",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Customers_CustomerId",
                schema: "shop",
                table: "Orders",
                column: "CustomerId",
                principalSchema: "shop",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_SalesAgents_SalesAgentId",
                schema: "shop",
                table: "Orders",
                column: "SalesAgentId",
                principalSchema: "shop",
                principalTable: "SalesAgents",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderLines_Products_ProductId",
                schema: "shop",
                table: "OrderLines");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderLines_Products_ProductId1",
                schema: "shop",
                table: "OrderLines");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Customers_CustomerId",
                schema: "shop",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_SalesAgents_SalesAgentId",
                schema: "shop",
                table: "Orders");

            migrationBuilder.DropTable(
                name: "CustomerProfiles",
                schema: "shop");

            migrationBuilder.DropTable(
                name: "ProductSuppliers",
                schema: "shop");

            migrationBuilder.DropTable(
                name: "ProductTags",
                schema: "shop");

            migrationBuilder.DropTable(
                name: "SalesAgents",
                schema: "shop");

            migrationBuilder.DropTable(
                name: "Suppliers",
                schema: "shop");

            migrationBuilder.DropTable(
                name: "Tags",
                schema: "shop");

            migrationBuilder.DropIndex(
                name: "IX_Orders_SalesAgentId",
                schema: "shop",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_OrderLines_ProductId1",
                schema: "shop",
                table: "OrderLines");

            migrationBuilder.DropColumn(
                name: "SalesAgentId",
                schema: "shop",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ProductId1",
                schema: "shop",
                table: "OrderLines");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderLines_Products_ProductId",
                schema: "shop",
                table: "OrderLines",
                column: "ProductId",
                principalSchema: "shop",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Customers_CustomerId",
                schema: "shop",
                table: "Orders",
                column: "CustomerId",
                principalSchema: "shop",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddFluentApi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderLines",
                table: "OrderLines");

            migrationBuilder.DropIndex(
                name: "IX_OrderLines_OrderId",
                table: "OrderLines");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "OrderLines");

            migrationBuilder.EnsureSchema(
                name: "shop");

            migrationBuilder.RenameTable(
                name: "Products",
                newName: "Products",
                newSchema: "shop");

            migrationBuilder.RenameTable(
                name: "Orders",
                newName: "Orders",
                newSchema: "shop");

            migrationBuilder.RenameTable(
                name: "OrderLines",
                newName: "OrderLines",
                newSchema: "shop");

            migrationBuilder.RenameTable(
                name: "Customers",
                newName: "Customers",
                newSchema: "shop");

            migrationBuilder.AlterColumn<string>(
                name: "Sku",
                schema: "shop",
                table: "Products",
                type: "varchar(32)",
                unicode: false,
                maxLength: 32,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "shop",
                table: "Products",
                type: "varchar(200)",
                unicode: false,
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "shop",
                table: "Customers",
                type: "varchar(200)",
                unicode: false,
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                schema: "shop",
                table: "Customers",
                type: "varchar(320)",
                unicode: false,
                maxLength: 320,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Products_Sku",
                schema: "shop",
                table: "Products",
                column: "Sku");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderLines",
                schema: "shop",
                table: "OrderLines",
                columns: new[] { "OrderId", "ProductId" });

            migrationBuilder.CreateIndex(
                name: "IX_Products_Sku",
                schema: "shop",
                table: "Products",
                column: "Sku");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Products_Price_NonNegative",
                schema: "shop",
                table: "Products",
                sql: "[Price] >= 0");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Email",
                schema: "shop",
                table: "Customers",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_Products_Sku",
                schema: "shop",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_Sku",
                schema: "shop",
                table: "Products");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Products_Price_NonNegative",
                schema: "shop",
                table: "Products");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderLines",
                schema: "shop",
                table: "OrderLines");

            migrationBuilder.DropIndex(
                name: "IX_Customers_Email",
                schema: "shop",
                table: "Customers");

            migrationBuilder.RenameTable(
                name: "Products",
                schema: "shop",
                newName: "Products");

            migrationBuilder.RenameTable(
                name: "Orders",
                schema: "shop",
                newName: "Orders");

            migrationBuilder.RenameTable(
                name: "OrderLines",
                schema: "shop",
                newName: "OrderLines");

            migrationBuilder.RenameTable(
                name: "Customers",
                schema: "shop",
                newName: "Customers");

            migrationBuilder.AlterColumn<string>(
                name: "Sku",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(32)",
                oldUnicode: false,
                oldMaxLength: 32);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldUnicode: false,
                oldMaxLength: 200);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "OrderLines",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldUnicode: false,
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(320)",
                oldUnicode: false,
                oldMaxLength: 320);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderLines",
                table: "OrderLines",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_OrderLines_OrderId",
                table: "OrderLines",
                column: "OrderId");
        }
    }
}

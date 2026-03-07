using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddValueObjects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PriceCurrency",
                schema: "shop",
                table: "Products",
                type: "char(3)",
                unicode: false,
                fixedLength: true,
                nullable: false,
                defaultValue: "USD");

            migrationBuilder.AddColumn<string>(
                name: "ShipToCity",
                schema: "shop",
                table: "Orders",
                type: "varchar(100)",
                unicode: false,
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ShipToCountryCode",
                schema: "shop",
                table: "Orders",
                type: "char(2)",
                unicode: false,
                fixedLength: true,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ShipToPostalCode",
                schema: "shop",
                table: "Orders",
                type: "varchar(20)",
                unicode: false,
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ShipToStreet",
                schema: "shop",
                table: "Orders",
                type: "varchar(200)",
                unicode: false,
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UnitPriceCurrency",
                schema: "shop",
                table: "OrderLines",
                type: "char(3)",
                unicode: false,
                fixedLength: true,
                nullable: false,
                defaultValue: "USD");

            migrationBuilder.AddColumn<string>(
                name: "BillCity",
                schema: "shop",
                table: "Customers",
                type: "varchar(100)",
                unicode: false,
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BillCountryCode",
                schema: "shop",
                table: "Customers",
                type: "char(2)",
                unicode: false,
                fixedLength: true,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BillPostalCode",
                schema: "shop",
                table: "Customers",
                type: "varchar(20)",
                unicode: false,
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BillStreet",
                schema: "shop",
                table: "Customers",
                type: "varchar(200)",
                unicode: false,
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "BillingAddress_BillPresent",
                schema: "shop",
                table: "Customers",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShipCity",
                schema: "shop",
                table: "Customers",
                type: "varchar(100)",
                unicode: false,
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ShipCountryCode",
                schema: "shop",
                table: "Customers",
                type: "char(2)",
                unicode: false,
                fixedLength: true,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ShipPostalCode",
                schema: "shop",
                table: "Customers",
                type: "varchar(20)",
                unicode: false,
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ShipStreet",
                schema: "shop",
                table: "Customers",
                type: "varchar(200)",
                unicode: false,
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PriceCurrency",
                schema: "shop",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ShipToCity",
                schema: "shop",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ShipToCountryCode",
                schema: "shop",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ShipToPostalCode",
                schema: "shop",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ShipToStreet",
                schema: "shop",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "UnitPriceCurrency",
                schema: "shop",
                table: "OrderLines");

            migrationBuilder.DropColumn(
                name: "BillCity",
                schema: "shop",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "BillCountryCode",
                schema: "shop",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "BillPostalCode",
                schema: "shop",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "BillStreet",
                schema: "shop",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "BillingAddress_BillPresent",
                schema: "shop",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "ShipCity",
                schema: "shop",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "ShipCountryCode",
                schema: "shop",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "ShipPostalCode",
                schema: "shop",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "ShipStreet",
                schema: "shop",
                table: "Customers");
        }
    }
}

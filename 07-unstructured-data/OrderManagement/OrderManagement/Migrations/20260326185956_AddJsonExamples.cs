using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddJsonExamples : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Catalog",
                schema: "shop",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "{}");

            migrationBuilder.AddColumn<string>(
                name: "Audit",
                schema: "shop",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "{}");

            migrationBuilder.AddColumn<string>(
                name: "Preferences",
                schema: "shop",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "{}");

            migrationBuilder.AddColumn<string>(
                name: "PreferencesTheme",
                schema: "shop",
                table: "Customers",
                type: "nvarchar(450)",
                nullable: true,
                computedColumnSql: "JSON_VALUE([Preferences], '$.Ui.Theme')",
                stored: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_PreferencesTheme",
                schema: "shop",
                table: "Customers",
                column: "PreferencesTheme");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Customers_PreferencesTheme",
                schema: "shop",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "PreferencesTheme",
                schema: "shop",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Catalog",
                schema: "shop",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Audit",
                schema: "shop",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Preferences",
                schema: "shop",
                table: "Customers");
        }
    }
}

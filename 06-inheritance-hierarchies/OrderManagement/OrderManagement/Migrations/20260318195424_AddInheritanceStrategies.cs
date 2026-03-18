using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddInheritanceStrategies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AutoRenews",
                schema: "shop",
                table: "Products",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BillingPeriodMonths",
                schema: "shop",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DimHeightCm",
                schema: "shop",
                table: "Products",
                type: "decimal(9,2)",
                precision: 9,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DimLengthCm",
                schema: "shop",
                table: "Products",
                type: "decimal(9,2)",
                precision: 9,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DimWidthCm",
                schema: "shop",
                table: "Products",
                type: "decimal(9,2)",
                precision: 9,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DownloadUrl",
                schema: "shop",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "FileSizeBytes",
                schema: "shop",
                table: "Products",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDiscontinued",
                schema: "shop",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Manufacturer",
                schema: "shop",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProductKind",
                schema: "shop",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "RequiresSignature",
                schema: "shop",
                table: "Products",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "TrialEndsOn",
                schema: "shop",
                table: "Products",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "WeightKg",
                schema: "shop",
                table: "Products",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Products_Digital_RequiresUrl",
                schema: "shop",
                table: "Products",
                sql: "([ProductKind] <> 2) OR ([DownloadUrl] IS NOT NULL AND [DownloadUrl] <> '')");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Products_Physical_RequiresWeight",
                schema: "shop",
                table: "Products",
                sql: "([ProductKind] <> 1) OR ([WeightKg] IS NOT NULL AND [WeightKg] > 0)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Products_Digital_RequiresUrl",
                schema: "shop",
                table: "Products");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Products_Physical_RequiresWeight",
                schema: "shop",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "AutoRenews",
                schema: "shop",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "BillingPeriodMonths",
                schema: "shop",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "DimHeightCm",
                schema: "shop",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "DimLengthCm",
                schema: "shop",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "DimWidthCm",
                schema: "shop",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "DownloadUrl",
                schema: "shop",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "FileSizeBytes",
                schema: "shop",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "IsDiscontinued",
                schema: "shop",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Manufacturer",
                schema: "shop",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ProductKind",
                schema: "shop",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "RequiresSignature",
                schema: "shop",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "TrialEndsOn",
                schema: "shop",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "WeightKg",
                schema: "shop",
                table: "Products");
        }
    }
}

using ContosoLegacySales.Data.Context;
using Microsoft.EntityFrameworkCore;
using ScaffoldedLegacyApp.Legacy;
using ScaffoldedLegacyApp.Modern;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<LegacySalesDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("LegacySales")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

using var scope = app.Services.CreateScope();

var db = scope.ServiceProvider.GetRequiredService<LegacySalesDbContext>();

Console.WriteLine();
Console.WriteLine("Legacy projection:");
var legacy = await LegacyProjectionExamples.GetOrderSummaries_LegacyPattern(db);
foreach (var item in legacy)
{
    Console.WriteLine($"{item.OrderNo} | {item.CustomerName} | {item.StatusName} | {item.SalesRepName} | {item.NetAmount}");
}

Console.WriteLine();
Console.WriteLine("Modern projection:");
var modern = await ModernProjectionExamples.GetOrderSummaries_ModernLeftJoin(db);
foreach (var item in modern)
{
    Console.WriteLine($"{item.OrderNo} | {item.CustomerName} | {item.StatusName} | {item.SalesRepName} | {item.NetAmount}");
}

Console.WriteLine();
Console.WriteLine("Modern right join:");
var rightJoin = await ModernProjectionExamples.GetAllProductsIncludingUnorderedOnes(db);
foreach (var item in rightJoin)
{
    Console.WriteLine($"{item.OrderNo} | {item.OrderId} | {item.SKU} | {item.ProductName}");
}

app.Run();

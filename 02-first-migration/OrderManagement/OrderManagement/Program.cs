using Microsoft.EntityFrameworkCore;
using OrderManagement.Data;
using OrderManagement.Domain;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope
        .ServiceProvider
        .GetRequiredService<AppDbContext>();

    await context.Database.MigrateAsync();
}

app.UseSwagger();
app.UseSwaggerUI();

// simple health
app.MapGet("/", () => Results.Ok(new { status = "ok", service = "OrderManagement" }));

// ---- Customers ----
app.MapPost("/customers", async (AppDbContext db, Customer customer) =>
{
    db.Customers.Add(customer);
    await db.SaveChangesAsync();
    return Results.Created($"/customers/{customer.Id}", customer);
});

app.MapGet("/customers", async (AppDbContext db) =>
    await db.Customers.AsNoTracking().OrderBy(c => c.Id).ToListAsync());

app.MapGet("/customers/{id:int}", async (AppDbContext db, int id) =>
{
    var customer = await db.Customers.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
    return customer is null ? Results.NotFound() : Results.Ok(customer);
});

// ---- Products ----
app.MapPost("/products", async (AppDbContext db, Product product) =>
{
    db.Products.Add(product);
    await db.SaveChangesAsync();
    return Results.Created($"/products/{product.Id}", product);
});

app.MapGet("/products", async (AppDbContext db) =>
    await db.Products.AsNoTracking().OrderBy(p => p.Id).ToListAsync());

// ---- Orders ----
// Request DTOs keep endpoints clean
app.MapPost("/orders", async (AppDbContext db, CreateOrderRequest request) =>
{
    var customerExists = await db.Customers.AnyAsync(c => c.Id == request.CustomerId);
    if (!customerExists) return Results.BadRequest($"Customer {request.CustomerId} not found.");

    // load product prices
    var productIds = request.Lines.Select(l => l.ProductId).Distinct().ToList();
    var products = await db.Products
        .Where(p => productIds.Contains(p.Id))
        .ToDictionaryAsync(p => p.Id);

    if (products.Count != productIds.Count)
        return Results.BadRequest("One or more products were not found.");

    var order = new Order
    {
        CustomerId = request.CustomerId,
        CreatedUtc = DateTime.UtcNow,
        Lines = request.Lines.Select(l => new OrderLine
        {
            ProductId = l.ProductId,
            Quantity = l.Quantity,
            UnitPrice = products[l.ProductId].Price
        }).ToList()
    };

    db.Orders.Add(order);
    await db.SaveChangesAsync();

    return Results.Created($"/orders/{order.Id}", new { order.Id, order.CustomerId, order.CreatedUtc });
});

app.MapGet("/orders/{id:int}", async (AppDbContext db, int id) =>
{
    var order = await db.Orders
        .AsNoTracking()
        .Include(o => o.Customer)
        .Include(o => o.Lines)
            .ThenInclude(l => l.Product)
        .FirstOrDefaultAsync(o => o.Id == id);

    if (order is null) return Results.NotFound();

    return Results.Ok(new
    {
        order.Id,
        order.CreatedUtc,
        Customer = new { order.Customer.Id, order.Customer.Name, order.Customer.Email },
        Lines = order.Lines.Select(l => new
        {
            l.Id,
            Product = new { l.Product.Id, l.Product.Sku, l.Product.Name },
            l.Quantity,
            l.UnitPrice,
            LineTotal = l.UnitPrice * l.Quantity
        }),
        Total = order.Lines.Sum(l => l.UnitPrice * l.Quantity)
    });
});

app.Run();

public record CreateOrderRequest(int CustomerId, List<CreateOrderLineRequest> Lines);
public record CreateOrderLineRequest(int ProductId, int Quantity);

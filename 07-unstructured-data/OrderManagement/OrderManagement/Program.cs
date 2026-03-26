using Microsoft.EntityFrameworkCore;
using OrderManagement.Data;
using OrderManagement.Domain;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
           .EnableDetailedErrors()
           .EnableSensitiveDataLogging()
           .LogTo(Console.WriteLine, LogLevel.Information));

if (builder.Configuration["Cosmos:Endpoint"] is not null)
{
    builder.Services.AddDbContext<OrderManagementCosmosContext>(options =>
        options.UseCosmos(
            builder.Configuration["Cosmos:Endpoint"]!,
            builder.Configuration["Cosmos:Key"]!,
            builder.Configuration["Cosmos:Database"]!));
}

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

app.MapGet("/customers", async (AppDbContext db) =>
    await db.Customers.AsNoTracking().OrderBy(c => c.Id).ToListAsync());

app.MapGet("/customers/{id:int}", async (AppDbContext db, int id) =>
{
    var customer = await db.Customers.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
    return customer is null ? Results.NotFound() : Results.Ok(customer);
});

app.MapPost("/products", async (AppDbContext db, Product product) =>
{
    db.Products.Add(product);
    await db.SaveChangesAsync();
    return Results.Created($"/products/{product.Id}", product);
});

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
            Product = new { l.Product.Id, l.Product.Sku, l.Product.Name },
            l.Quantity,
            l.UnitPrice,
            LineTotal = l.UnitPrice.Amount * l.Quantity
        }),
        Total = order.Lines.Sum(l => l.UnitPrice.Amount * l.Quantity)
    });
});

app.MapPost("/products/{productId:int}/tags/{tagId:int}", async (AppDbContext db, int productId, int tagId) =>
{
    var product = await db.Products.Include(p => p.Tags).FirstAsync(p => p.Id == productId);
    var tag = await db.Tags.FirstAsync(t => t.Id == tagId);

    product.Tags.Add(tag);
    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.MapPost("/products/{productId:int}/suppliers/{supplierId:int}", async (AppDbContext db, int productId, int supplierId, ProductSupplier link) =>
{
    link.ProductId = productId;
    link.SupplierId = supplierId;

    db.ProductSuppliers.Add(link);
    await db.SaveChangesAsync();

    return Results.Created($"/products/{productId}/suppliers/{supplierId}", link);
});

app.MapGet("/orders/{orderId:int}/agent", async (AppDbContext db, int orderId) =>
{
    var order = await db.Orders.FirstAsync(o => o.Id == orderId);
    var agentId = db.Entry(order).Property<int?>("SalesAgentId").OriginalValue;
    return Results.Ok(agentId);
});

app.MapPost("/orders/{orderId:int}/assign-agent/{agentId:int}", async (AppDbContext db, int orderId, int agentId) =>
{
    var order = await db.Orders.FirstAsync(o => o.Id == orderId);
    db.Entry(order).Property<int?>("SalesAgentId").CurrentValue = agentId;
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapPost("/products/physical", async (AppDbContext db, PhysicalProduct p) =>
{
    db.PhysicalProducts.Add(p);
    await db.SaveChangesAsync();
    return Results.Created($"/products/{p.Id}", p);
});

app.MapPost("/products/digital", async (AppDbContext db, DigitalProduct p) =>
{
    db.DigitalProducts.Add(p);
    await db.SaveChangesAsync();
    return Results.Created($"/products/{p.Id}", p);
});

app.MapPost("/products/subscription", async (AppDbContext db, SubscriptionProduct p) =>
{
    db.SubscriptionProducts.Add(p);
    await db.SaveChangesAsync();
    return Results.Created($"/products/{p.Id}", p);
});

app.MapGet("/products", async (AppDbContext db) =>
    await db.Products.AsNoTracking().OrderBy(p => p.Id).ToListAsync());

app.MapGet("/products/physical", async (AppDbContext db) =>
    await db.Products.OfType<PhysicalProduct>().AsNoTracking().OrderBy(p => p.Id).ToListAsync());

// Create customer with JSON preferences
app.MapPost("/customers", async (AppDbContext db, Customer customer) =>
{
    db.Customers.Add(customer);
    await db.SaveChangesAsync();
    return Results.Created($"/customers/{customer.Id}", customer);
});

// Query by JSON property
app.MapGet("/customers/by-theme/{theme}", async (AppDbContext db, string theme) =>
    await db.Customers.AsNoTracking()
        .Where(c => c.Preferences.Ui.Theme == theme)
        .OrderBy(c => c.Id)
        .ToListAsync());

// Bulk update JSON property (ExecuteUpdate)
app.MapPost("/customers/theme/bulk/{theme}", async (AppDbContext db, string theme) =>
{
    await db.Customers.ExecuteUpdateAsync(s =>
        s.SetProperty(c => c.Preferences.Ui.Theme, theme));
    return Results.NoContent();
});

// Append an audit event (best practice: load → modify → SaveChanges)
app.MapPost("/orders/{id:int}/audit", async (AppDbContext db, int id, OrderAuditEvent ev) =>
{
    var order = await db.Orders.SingleAsync(o => o.Id == id);

    var events = order.Audit.Events.ToList();
    events.Add(ev);

    order.Audit.Events = events.ToArray();
    order.Audit.LastEventType = ev.Type;

    await db.SaveChangesAsync();
    return Results.Ok(order.Audit);
});

// Cosmos
app.MapPost("/cosmos/orders", async (OrderManagementCosmosContext db, CosmosOrderDocument doc) =>
{
    db.Orders.Add(doc);
    await db.SaveChangesAsync();
    return Results.Created($"/cosmos/orders/{doc.Id}", doc);
});

app.MapGet("/cosmos/orders/by-customer/{customerId}", async (OrderManagementCosmosContext db, string customerId) =>
    await db.Orders.Where(o => o.CustomerId == customerId).ToListAsync());

app.Run();

public record CreateOrderRequest(int CustomerId, List<CreateOrderLineRequest> Lines);
public record CreateOrderLineRequest(int ProductId, int Quantity);

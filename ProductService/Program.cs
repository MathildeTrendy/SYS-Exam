using System.Collections.Concurrent;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Simpel in-memory "database"
var products = new ConcurrentDictionary<string, Product>();

// Produkter
products["p1"] = new Product("p1", "Håndbold", 5);
products["p2"] = new Product("p2", "Håndboldsko", 2);

// GET /products -> alle produkter
app.MapGet("/products", () =>
{
    return Results.Ok(products.Values);
});

// GET /products/{id} -> ét produkt
app.MapGet("/products/{id}", (string id) =>
{
    if (products.TryGetValue(id, out var product))
    {
        return Results.Ok(product);
    }

    return Results.NotFound(new { error = "PRODUCT_NOT_FOUND" });
});

// POST /products/{id}/reserve -> reserverer lager
// Kontrakten, som OrderService forventer
app.MapPost("/products/{id}/reserve", (string id, ReserveRequest request) =>
{
    if (!products.TryGetValue(id, out var product))
    {
        return Results.NotFound(new { error = "PRODUCT_NOT_FOUND" });
    }

    if (request.Quantity <= 0)
    {
        return Results.BadRequest(new { error = "INVALID_QUANTITY" });
    }

    if (product.Stock < request.Quantity)
    {
        return Results.BadRequest(new ReserveResponse(false, "OUT_OF_STOCK"));
    }

    // Opdater lager
    var updated = product with { Stock = product.Stock - request.Quantity };
    products[id] = updated;

    return Results.Ok(new ReserveResponse(true, null));
});

app.Run();

// Models / DTO'er
public record Product(string Id, string Name, int Stock);

public record ReserveRequest(int Quantity);

public record ReserveResponse(bool Success, string? Reason);

public partial class Program { }

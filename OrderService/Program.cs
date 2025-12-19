using System.Collections.Concurrent;
using System.Net.Http.Json;

var builder = WebApplication.CreateBuilder(args);

// HttpClient kalder ProductService
builder.Services.AddHttpClient("ProductService", client =>
{
    client.BaseAddress = new Uri("http://localhost:5203");
});

var app = builder.Build();

// In-memory database til ordrer
var orders = new ConcurrentDictionary<int, Order>();
var nextOrderId = 1;

// Opret ordre (kalder ProductService)
app.MapPost("/orders", async (CreateOrderRequest request, IHttpClientFactory httpClientFactory) =>
{
    var client = httpClientFactory.CreateClient("ProductService");

    // Gør request klar til ProductService
    var reserveRequest = new ReserveRequest(request.Quantity);

    // Kald ProductService
    var response = await client.PostAsJsonAsync($"/products/{request.ProductId}/reserve", reserveRequest);

    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
    {
        return Results.BadRequest(new { error = "PRODUCT_NOT_FOUND" });
    }

    // Læser svar fra ProductService
    var reserveResponse = await response.Content.ReadFromJsonAsync<ReserveResponse>();

    // Hvis noget gik galt eller ikke success
    if (!response.IsSuccessStatusCode || reserveResponse is null || !reserveResponse.Success)
    {
        var reason = reserveResponse?.Reason ?? "RESERVATION_FAILED";
        return Results.BadRequest(new { error = reason });
    }

    // Hvis vi når hertil, er der reserveret lager = opret ordre
    var orderId = nextOrderId++;
    var order = new Order(orderId, request.ProductId, request.Quantity, "CREATED");
    orders[orderId] = order;

    return Results.Ok(order);
});

// Hent ordre
app.MapGet("/orders/{id}", (int id) =>
{
    if (orders.TryGetValue(id, out var order))
    {
        return Results.Ok(order);
    }

    return Results.NotFound(new { error = "ORDER_NOT_FOUND" });
});

app.Run();

// Models
public record Order(int Id, string ProductId, int Quantity, string Status);

public record CreateOrderRequest(string ProductId, int Quantity);

// Skal matche kontrakten fra ProductService
public record ReserveRequest(int Quantity);

public record ReserveResponse(bool Success, string? Reason);

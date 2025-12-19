using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace ProductService.ContractTests;

public class ReserveEndpointTests
{
    private readonly HttpClient _client = new()
    {
        BaseAddress = new Uri("http://localhost:5203")
    };

    [Fact]
    public async Task Reserve_ReturnsSuccessTrue_WhenEnoughStock()
    {
        // Arrange
        var request = new ReserveRequest(1);

        // Act
        var response = await _client.PostAsJsonAsync("/products/p1/reserve", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<ReserveResponse>();
        Assert.NotNull(body);
        Assert.True(body!.Success);
        Assert.Null(body.Reason);
    }

    [Fact]
    public async Task Reserve_ReturnsOutOfStock_WhenNotEnoughStock()
    {
        // Arrange
        var request = new ReserveRequest(999);

        // Act
        var response = await _client.PostAsJsonAsync("/products/p1/reserve", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<ReserveResponse>();
        Assert.NotNull(body);
        Assert.False(body!.Success);
        Assert.Equal("OUT_OF_STOCK", body.Reason);
    }
}

public record ReserveRequest(int Quantity);
public record ReserveResponse(bool Success, string? Reason);

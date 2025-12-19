using OrderService;
using Xunit;

namespace OrderService.UnitTests;

public class ErrorMapperTests
{
    [Fact]
    public void MapReserveError_ReturnsOutOfStock_WhenReasonIsOutOfStock()
    {
        // Arrange
        var reserveResponse = new ReserveResponse(false, "OUT_OF_STOCK");

        // Act
        var result = ErrorMapper.MapReserveError(reserveResponse);

        // Assert
        Assert.Equal("OUT_OF_STOCK", result);
    }
}


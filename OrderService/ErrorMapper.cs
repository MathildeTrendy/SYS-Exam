namespace OrderService;

public static class ErrorMapper
{
    public static string MapReserveError(ReserveResponse? reserveResponse)
    {
        if (reserveResponse is null || !reserveResponse.Success)
        {
            return reserveResponse?.Reason ?? "RESERVATION_FAILED";
        }
        return string.Empty;
    }
}

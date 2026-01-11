namespace OsService.Web.ApiClients;

public class ServiceOrdersApiClient(HttpClient httpClient)
{
    public async Task<ServiceOrderDto[]> GetServiceOrdersAsync(CancellationToken cancellationToken = default)
    {
        return await httpClient.GetFromJsonAsync<ServiceOrderDto[]>("/api/v1/serviceorders", cancellationToken) ?? [];
    }

    public async Task<OpenServiceOrderResponse> OpenServiceOrderAsync(OpenServiceOrderRequest request, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync("/api/v1/serviceorders", request, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<OpenServiceOrderResponse>(cancellationToken) 
            ?? throw new InvalidOperationException("Failed to create service order");
    }

    public async Task UpdateStatusAsync(Guid serviceOrderId, int status, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PatchAsJsonAsync($"/api/v1/serviceorders/{serviceOrderId}/status", 
            new { serviceOrderId, newStatus = status }, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task UpdatePriceAsync(Guid serviceOrderId, decimal price, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PutAsJsonAsync($"/api/v1/serviceorders/{serviceOrderId}/price", 
            new { serviceOrderId, price }, cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}

public record ServiceOrderDto(
Guid Id, 
int Number, 
Guid CustomerId,
string CustomerName,
string Description, 
int Status,
DateTime OpenedAt, 
DateTime? StartedAt, 
DateTime? FinishedAt, 
decimal? Price, 
string? Coin, 
DateTime? UpdatedPriceAt);

public class OpenServiceOrderRequest
{
    public Guid CustomerId { get; set; }
    public string Description { get; set; } = string.Empty;
}

public record OpenServiceOrderResponse(Guid Id, int Number);

namespace OsService.Web.ApiClients;

public class CustomersApiClient(HttpClient httpClient)
{
    public async Task<CustomerDto[]> GetCustomersAsync(CancellationToken cancellationToken = default)
    {
        return await httpClient.GetFromJsonAsync<CustomerDto[]>("/api/v1/customers", cancellationToken) ?? [];
    }

    public async Task<Guid> CreateCustomerAsync(CreateCustomerRequest request, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync("/api/v1/customers", request, cancellationToken);
        response.EnsureSuccessStatusCode();
        
        var result = await response.Content.ReadFromJsonAsync<CreateCustomerResponse>(cancellationToken);
        return result?.Id ?? throw new InvalidOperationException("Falha ao criar cliente.");
    }
}

public record CustomerDto(Guid Id, string Name, string? Phone, string? Email, string? Document, DateTime CreatedAt);

public class CreateCustomerRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Document { get; set; }
}

public record CreateCustomerResponse(Guid Id);


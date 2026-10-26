/*Author: Zenwyn Naicker*/
/*Code Attribution: */
/**************************************************************************************************************
 * Author: xUnit.net
 * Date: 2025
 * Title: Getting Started with xUnit.net v2
 * Type: [Source Code]
 * Available at: <https://xunit.net/docs/getting-started/v2/getting-started> [Accessed 6 April 2026]
 **************************************************************************************************************/
/**************************************************************************************************************
 * Author: Microsoft
 * Date: 2026
 * Title: Integration tests in ASP.NET Core
 * Type: [Source Code]
 * Available at: <https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-10.0> [Accessed 18 May 2026]
 **************************************************************************************************************/
/**************************************************************************************************************
 * Author: Microsoft
 * Date: 2023
 * Title: Testing without your production database system
 * Type: [Source Code]
 * Available at: <https://learn.microsoft.com/en-us/ef/core/testing/testing-without-the-database> [Accessed 6 April 2026]
 **************************************************************************************************************/
/**************************************************************************************************************
 * Author: Microsoft
 * Date: 2026
 * Title: System.Net.Http.Json Namespace
 * Type: [Source Code]
 * Available at: <https://learn.microsoft.com/en-us/dotnet/api/system.net.http.json?view=net-10.0> [Accessed 18 May 2026]
 **************************************************************************************************************/

using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace GLMS.Tests.Integration;

public class GlmsApiIntegrationTests : IDisposable
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly HttpClient _client;

    public GlmsApiIntegrationTests()
    {
        var baseUrl = Environment.GetEnvironmentVariable("GLMS_API_BASE_URL")
            ?? "http://localhost:5172/";

        _client = CreateHttpClient(baseUrl);
    }

    [Fact]
    public async Task Health_endpoint_returns_ok()
    {
        using var response = await _client.GetAsync("api/health");

        response.EnsureSuccessStatusCode();

        using var document = await ReadJsonAsync(response);
        Assert.Equal("Healthy", document.RootElement.GetProperty("status").GetString());
    }

    [Fact]
    public async Task Api_workflow_covers_auth_clients_contracts_currency_agreements_and_service_requests()
    {
        var token = await LoginAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        using var unauthenticatedClient = CreateHttpClient(_client.BaseAddress!.ToString());
        using var unauthorizedResponse = await unauthenticatedClient.GetAsync("api/clients");

        Assert.Equal(HttpStatusCode.Unauthorized, unauthorizedResponse.StatusCode);

        var unique = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        var clientId = await CreateClientAsync(unique);
        var contractId = await CreateContractAsync(clientId);
        await UpdateContractStatusAsync(contractId, 1);

        await AssertCurrencyPreviewAsync();
        await CreateServiceRequestAsync(contractId);

        await UploadAgreementAsync(contractId);
        await DownloadAgreementAsync(contractId);
        await RejectInvalidAgreementAsync(contractId);
    }

    private static HttpClient CreateHttpClient(string baseUrl)
    {
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };

        return new HttpClient(handler)
        {
            BaseAddress = new Uri(baseUrl)
        };
    }

    private async Task<string> LoginAsync()
    {
        using var response = await _client.PostAsJsonAsync("api/auth/login", new
        {
            username = Environment.GetEnvironmentVariable("GLMS_TEST_USERNAME")
                ?? throw new InvalidOperationException("Set GLMS_TEST_USERNAME for integration tests."),
            password = Environment.GetEnvironmentVariable("GLMS_TEST_PASSWORD")
                ?? throw new InvalidOperationException("Set GLMS_TEST_PASSWORD for integration tests.")
        }, JsonOptions);

        response.EnsureSuccessStatusCode();

        using var document = await ReadJsonAsync(response);
        var token = document.RootElement.GetProperty("token").GetString();

        Assert.False(string.IsNullOrWhiteSpace(token));

        return token;
    }

    private async Task<int> CreateClientAsync(long unique)
    {
        using var response = await _client.PostAsJsonAsync("api/clients", new
        {
            name = $"Integration Client {unique}",
            contactDetails = $"integration-{unique}@glms.test",
            region = "Gauteng"
        }, JsonOptions);

        response.EnsureSuccessStatusCode();

        using var document = await ReadJsonAsync(response);
        var clientId = document.RootElement.GetProperty("id").GetInt32();

        Assert.True(clientId > 0);

        using var getResponse = await _client.GetAsync("api/clients");
        getResponse.EnsureSuccessStatusCode();

        using var clientsDocument = await ReadJsonAsync(getResponse);
        Assert.Contains(clientsDocument.RootElement.EnumerateArray(), client =>
            client.GetProperty("id").GetInt32() == clientId);

        return clientId;
    }

    private async Task<int> CreateContractAsync(int clientId)
    {
        using var response = await _client.PostAsJsonAsync("api/contracts", new
        {
            clientId,
            startDate = DateTime.Today,
            endDate = DateTime.Today.AddYears(1),
            status = 0,
            serviceLevel = "Premium Logistics",
            signedAgreementPath = (string?)null
        }, JsonOptions);

        response.EnsureSuccessStatusCode();

        using var document = await ReadJsonAsync(response);
        var contractId = document.RootElement.GetProperty("id").GetInt32();

        Assert.True(contractId > 0);
        Assert.Equal(clientId, document.RootElement.GetProperty("clientId").GetInt32());

        return contractId;
    }

    private async Task UpdateContractStatusAsync(int contractId, int status)
    {
        using var response = await SendJsonAsync(HttpMethod.Patch, $"api/contracts/{contractId}/status", new
        {
            status
        });

        response.EnsureSuccessStatusCode();

        using var document = await ReadJsonAsync(response);
        Assert.Equal(status, document.RootElement.GetProperty("status").GetInt32());
    }

    private async Task AssertCurrencyPreviewAsync()
    {
        using var response = await _client.GetAsync("api/currency/preview?amountUsd=100");

        response.EnsureSuccessStatusCode();

        using var document = await ReadJsonAsync(response);
        Assert.Equal(100m, document.RootElement.GetProperty("amountUsd").GetDecimal());
        Assert.True(document.RootElement.GetProperty("amountZar").GetDecimal() > 0);
        Assert.False(string.IsNullOrWhiteSpace(document.RootElement.GetProperty("strategy").GetString()));
    }

    private async Task CreateServiceRequestAsync(int contractId)
    {
        using var response = await _client.PostAsJsonAsync("api/service-requests", new
        {
            contractId,
            description = "Integration test service request",
            costUsd = 100m
        }, JsonOptions);

        response.EnsureSuccessStatusCode();

        using var document = await ReadJsonAsync(response);
        Assert.Equal(contractId, document.RootElement.GetProperty("contractId").GetInt32());
        Assert.True(document.RootElement.GetProperty("costZar").GetDecimal() > 0);
    }

    private async Task UploadAgreementAsync(int contractId)
    {
        using var form = new MultipartFormDataContent();
        var pdfBytes = Encoding.UTF8.GetBytes("%PDF-1.4\n% GLMS integration test PDF\n%%EOF");

        var fileContent = new ByteArrayContent(pdfBytes);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

        form.Add(fileContent, "file", "agreement.pdf");

        using var response = await _client.PostAsync($"api/contracts/{contractId}/agreement", form);

        response.EnsureSuccessStatusCode();

        using var document = await ReadJsonAsync(response);
        Assert.Equal(contractId, document.RootElement.GetProperty("contractId").GetInt32());
        Assert.EndsWith(".pdf", document.RootElement.GetProperty("fileName").GetString());
    }

    private async Task DownloadAgreementAsync(int contractId)
    {
        using var response = await _client.GetAsync($"api/contracts/{contractId}/agreement");

        response.EnsureSuccessStatusCode();

        Assert.Equal("application/pdf", response.Content.Headers.ContentType?.MediaType);

        var bytes = await response.Content.ReadAsByteArrayAsync();
        Assert.NotEmpty(bytes);
    }

    private async Task RejectInvalidAgreementAsync(int contractId)
    {
        using var form = new MultipartFormDataContent();
        var invalidContent = new ByteArrayContent(Encoding.UTF8.GetBytes("not a pdf"));

        invalidContent.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
        form.Add(invalidContent, "file", "agreement.txt");

        using var response = await _client.PostAsync($"api/contracts/{contractId}/agreement", form);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private async Task<HttpResponseMessage> SendJsonAsync(HttpMethod method, string path, object payload)
    {
        var json = JsonSerializer.Serialize(payload, JsonOptions);

        using var request = new HttpRequestMessage(method, path)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        return await _client.SendAsync(request);
    }

    private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)
    {
        var json = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(json);
    }

    public void Dispose()
    {
        _client.Dispose();
    }
}

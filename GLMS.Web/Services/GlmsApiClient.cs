/*Author: Zenwyn Naicker*/
/*Code Attribution: */
/**************************************************************************************************************
 * Author: Microsoft
 * Date: 2025
 * Title: Use the IHttpClientFactory
 * Type: [Source Code]
 * Available at: <https://learn.microsoft.com/en-us/dotnet/core/extensions/httpclient-factory> [Accessed 6 April 2026]
 **************************************************************************************************************/
/**************************************************************************************************************
 * Author: Microsoft
 * Date: 2026
 * Title: System.Net.Http.Json Namespace
 * Type: [Source Code]
 * Available at: <https://learn.microsoft.com/en-us/dotnet/api/system.net.http.json?view=net-10.0> [Accessed 18 May 2026]
 **************************************************************************************************************/
/**************************************************************************************************************
 * Author: Microsoft
 * Date: 2025
 * Title: Upload files in ASP.NET Core
 * Type: [Source Code]
 * Available at: <https://learn.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads?view=aspnetcore-10.0> [Accessed 6 April 2026]
 **************************************************************************************************************/

using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using GLMS.Web.ViewModels;
using Microsoft.Extensions.Options;

namespace GLMS.Web.Services;

public class GlmsApiClient : IGlmsApiClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ApiSettings _settings;
    private string? _token;
    private DateTime _tokenExpiresAtUtc = DateTime.MinValue;

    public GlmsApiClient(HttpClient httpClient, IOptions<ApiSettings> settings)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
        _jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
    }

    public async Task<List<ClientViewModel>> GetClientsAsync()
    {
        return await GetAsync<List<ClientViewModel>>("api/clients") ?? new List<ClientViewModel>();
    }

    public async Task<ClientViewModel?> GetClientAsync(int id)
    {
        return await GetNullableAsync<ClientViewModel>($"api/clients/{id}");
    }

    public async Task<ClientViewModel> CreateClientAsync(CreateClientViewModel model)
    {
        return await SendJsonAsync<ClientViewModel>(HttpMethod.Post, "api/clients", model);
    }

    public async Task<List<ContractViewModel>> GetContractsAsync(DateTime? startDate, DateTime? endDate, ContractStatusViewModel? status)
    {
        var query = new List<string>();

        if (startDate.HasValue)
            query.Add($"startDate={startDate.Value:yyyy-MM-dd}");

        if (endDate.HasValue)
            query.Add($"endDate={endDate.Value:yyyy-MM-dd}");

        if (status.HasValue)
            query.Add($"status={Uri.EscapeDataString(status.Value.ToString())}");

        var path = query.Count == 0
            ? "api/contracts"
            : $"api/contracts?{string.Join("&", query)}";

        return await GetAsync<List<ContractViewModel>>(path) ?? new List<ContractViewModel>();
    }

    public async Task<ContractViewModel?> GetContractAsync(int id)
    {
        var contract = await GetNullableAsync<ContractViewModel>($"api/contracts/{id}");

        if (contract is null)
            return null;

        contract.ServiceRequests = await GetServiceRequestsAsync(id);
        return contract;
    }

    public async Task<ContractViewModel> CreateContractAsync(CreateContractViewModel model)
    {
        var payload = new
        {
            model.ClientId,
            model.StartDate,
            model.EndDate,
            model.Status,
            model.ServiceLevel,
            SignedAgreementPath = (string?)null
        };

        return await SendJsonAsync<ContractViewModel>(HttpMethod.Post, "api/contracts", payload);
    }

    public async Task<ContractViewModel?> UpdateContractStatusAsync(int id, ContractStatusViewModel status)
    {
        var payload = new
        {
            Status = status
        };

        return await SendJsonAsync<ContractViewModel>(HttpMethod.Patch, $"api/contracts/{id}/status", payload);
    }

    public async Task UploadAgreementAsync(int contractId, IFormFile file)
    {
        await EnsureAuthenticatedAsync();

        using var content = new MultipartFormDataContent();
        await using var stream = file.OpenReadStream();

        var fileContent = new StreamContent(stream);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(
            string.IsNullOrWhiteSpace(file.ContentType) ? "application/pdf" : file.ContentType);

        content.Add(fileContent, "file", file.FileName);

        using var request = new HttpRequestMessage(HttpMethod.Post, $"api/contracts/{contractId}/agreement")
        {
            Content = content
        };

        using var response = await _httpClient.SendAsync(request);
        await EnsureSuccessAsync(response);
    }

    public async Task<AgreementDownload?> DownloadAgreementAsync(int contractId)
    {
        await EnsureAuthenticatedAsync();

        using var response = await _httpClient.GetAsync($"api/contracts/{contractId}/agreement");

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        await EnsureSuccessAsync(response);

        var content = await response.Content.ReadAsByteArrayAsync();
        var contentType = response.Content.Headers.ContentType?.MediaType ?? "application/pdf";
        var fileName = response.Content.Headers.ContentDisposition?.FileNameStar
            ?? response.Content.Headers.ContentDisposition?.FileName?.Trim('"')
            ?? $"contract-{contractId}-agreement.pdf";

        return new AgreementDownload(content, contentType, fileName);
    }

    public async Task<List<ServiceRequestViewModel>> GetServiceRequestsAsync(int? contractId = null)
    {
        var path = contractId.HasValue
            ? $"api/service-requests?contractId={contractId.Value}"
            : "api/service-requests";

        return await GetAsync<List<ServiceRequestViewModel>>(path) ?? new List<ServiceRequestViewModel>();
    }

    public async Task<ServiceRequestViewModel> CreateServiceRequestAsync(CreateServiceRequestViewModel model)
    {
        return await SendJsonAsync<ServiceRequestViewModel>(HttpMethod.Post, "api/service-requests", model);
    }

    public async Task<CurrencyRateViewModel> GetCurrencyRateAsync()
    {
        return await GetAsync<CurrencyRateViewModel>("api/currency/usd-to-zar")
            ?? new CurrencyRateViewModel();
    }

    public async Task<CurrencyPreviewViewModel> PreviewCurrencyAsync(decimal amountUsd)
    {
        var amount = amountUsd.ToString(CultureInfo.InvariantCulture);

        return await GetAsync<CurrencyPreviewViewModel>($"api/currency/preview?amountUsd={amount}")
            ?? new CurrencyPreviewViewModel { AmountUsd = amountUsd };
    }

    private async Task<T?> GetAsync<T>(string path)
    {
        await EnsureAuthenticatedAsync();

        using var response = await _httpClient.GetAsync(path);
        await EnsureSuccessAsync(response);

        return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
    }

    private async Task<T?> GetNullableAsync<T>(string path)
    {
        await EnsureAuthenticatedAsync();

        using var response = await _httpClient.GetAsync(path);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return default;

        await EnsureSuccessAsync(response);

        return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
    }

    private async Task<T> SendJsonAsync<T>(HttpMethod method, string path, object payload)
    {
        await EnsureAuthenticatedAsync();

        using var request = new HttpRequestMessage(method, path)
        {
            Content = JsonContent.Create(payload, options: _jsonOptions)
        };

        using var response = await _httpClient.SendAsync(request);
        await EnsureSuccessAsync(response);

        var result = await response.Content.ReadFromJsonAsync<T>(_jsonOptions);

        return result ?? throw new ApiClientException(HttpStatusCode.InternalServerError, "The API returned an empty response.");
    }

    private async Task EnsureAuthenticatedAsync()
    {
        if (!string.IsNullOrWhiteSpace(_token) && DateTime.UtcNow < _tokenExpiresAtUtc.AddMinutes(-1))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            return;
        }

        var login = new
        {
            Username = _settings.Username,
            Password = _settings.Password
        };

        using var response = await _httpClient.PostAsJsonAsync("api/auth/login", login, _jsonOptions);
        await EnsureSuccessAsync(response);

        var loginResult = await response.Content.ReadFromJsonAsync<LoginResponse>(_jsonOptions)
            ?? throw new ApiClientException(HttpStatusCode.Unauthorized, "The API did not return a token.");

        _token = loginResult.Token;
        _tokenExpiresAtUtc = loginResult.ExpiresAtUtc;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
            return;

        var message = await ReadErrorMessageAsync(response);
        throw new ApiClientException(response.StatusCode, message);
    }

    private static async Task<string> ReadErrorMessageAsync(HttpResponseMessage response)
    {
        var message = await response.Content.ReadAsStringAsync();

        if (string.IsNullOrWhiteSpace(message))
            return response.ReasonPhrase ?? "API request failed.";

        try
        {
            using var document = JsonDocument.Parse(message);

            if (document.RootElement.TryGetProperty("message", out var messageProperty))
                return messageProperty.GetString() ?? message;
        }
        catch (JsonException)
        {
            return message;
        }

        return message;
    }

    private class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAtUtc { get; set; }
    }
}

public class ApiClientException : Exception
{
    public ApiClientException(HttpStatusCode statusCode, string message) : base(message)
    {
        StatusCode = statusCode;
    }

    public HttpStatusCode StatusCode { get; }
}

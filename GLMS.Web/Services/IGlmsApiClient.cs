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
 * Date: 2025
 * Title: Upload files in ASP.NET Core
 * Type: [Source Code]
 * Available at: <https://learn.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads?view=aspnetcore-10.0> [Accessed 6 April 2026]
 **************************************************************************************************************/

using GLMS.Web.ViewModels;
using Microsoft.AspNetCore.Http;

namespace GLMS.Web.Services;

public interface IGlmsApiClient
{
    Task<List<ClientViewModel>> GetClientsAsync();
    Task<ClientViewModel?> GetClientAsync(int id);
    Task<ClientViewModel> CreateClientAsync(CreateClientViewModel model);

    Task<List<ContractViewModel>> GetContractsAsync(DateTime? startDate, DateTime? endDate, ContractStatusViewModel? status);
    Task<ContractViewModel?> GetContractAsync(int id);
    Task<ContractViewModel> CreateContractAsync(CreateContractViewModel model);
    Task<ContractViewModel?> UpdateContractStatusAsync(int id, ContractStatusViewModel status);
    Task UploadAgreementAsync(int contractId, IFormFile file);
    Task<AgreementDownload?> DownloadAgreementAsync(int contractId);

    Task<List<ServiceRequestViewModel>> GetServiceRequestsAsync(int? contractId = null);
    Task<ServiceRequestViewModel> CreateServiceRequestAsync(CreateServiceRequestViewModel model);

    Task<CurrencyRateViewModel> GetCurrencyRateAsync();
    Task<CurrencyPreviewViewModel> PreviewCurrencyAsync(decimal amountUsd);
}

public record AgreementDownload(byte[] Content, string ContentType, string FileName);

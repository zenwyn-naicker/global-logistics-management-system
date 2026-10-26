using GLMS.Api.Dtos;
using Microsoft.AspNetCore.Http;

namespace GLMS.Api.Services;

public interface IAgreementService
{
    Task<AgreementUploadResultDto> UploadAgreementAsync(int contractId, IFormFile file);
    Task<AgreementDto?> GetAgreementAsync(int contractId);
    Task<(string FilePath, string ContentType, string DownloadName)?> GetAgreementFileAsync(int contractId);
}

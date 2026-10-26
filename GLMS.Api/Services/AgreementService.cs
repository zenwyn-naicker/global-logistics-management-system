/*Author: Zenwyn Naicker*/
/*Code Attribution: */
/**************************************************************************************************************
 * Author: Microsoft
 * Date: 2025
 * Title: Upload files in ASP.NET Core
 * Type: [Source Code]
 * Available at: <https://learn.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads?view=aspnetcore-10.0> [Accessed 6 April 2026]
 **************************************************************************************************************/
/**************************************************************************************************************
 * Author: Microsoft
 * Date: 2022
 * Title: Implementing the Repository and Unit of Work Patterns in an ASP.NET MVC Application (9 of 10)
 * Type: [Source Code]
 * Available at: <https://learn.microsoft.com/en-us/aspnet/mvc/overview/older-versions/getting-started-with-ef-5-using-mvc-4/implementing-the-repository-and-unit-of-work-patterns-in-an-asp-net-mvc-application> [Accessed 18 May 2026]
 **************************************************************************************************************/

using GLMS.Api.Dtos;
using GLMS.Api.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace GLMS.Api.Services;

public class AgreementService : IAgreementService
{
    private readonly IContractRepository _contracts;
    private readonly IWebHostEnvironment _environment;

    public AgreementService(IContractRepository contracts, IWebHostEnvironment environment)
    {
        _contracts = contracts;
        _environment = environment;
    }

    public async Task<AgreementUploadResultDto> UploadAgreementAsync(int contractId, IFormFile file)
    {
        var contract = await _contracts.GetByIdAsync(contractId);

        if (contract is null)
            throw new ArgumentException("Contract not found.");

        if (file.Length == 0)
            throw new InvalidOperationException("Agreement file cannot be empty.");

        var extension = Path.GetExtension(file.FileName);

        if (!extension.Equals(".pdf", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Only PDF files are allowed.");

        var uploadRoot = Path.Combine(_environment.ContentRootPath, "uploads", "agreements");
        Directory.CreateDirectory(uploadRoot);

        var fileName = $"{Guid.NewGuid()}.pdf";
        var physicalPath = Path.Combine(uploadRoot, fileName);

        await using (var stream = new FileStream(physicalPath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        contract.SignedAgreementPath = Path.Combine("uploads", "agreements", fileName)
            .Replace('\\', '/');

        await _contracts.UpdateAsync(contract);

        return new AgreementUploadResultDto
        {
            ContractId = contract.Id,
            FileName = fileName,
            FilePath = contract.SignedAgreementPath
        };
    }

    public async Task<AgreementDto?> GetAgreementAsync(int contractId)
    {
        var contract = await _contracts.GetByIdAsync(contractId);

        if (contract is null)
            return null;

        return new AgreementDto
        {
            ContractId = contract.Id,
            SignedAgreementPath = contract.SignedAgreementPath
        };
    }

    public async Task<(string FilePath, string ContentType, string DownloadName)?> GetAgreementFileAsync(int contractId)
    {
        var contract = await _contracts.GetByIdAsync(contractId);

        if (contract is null || string.IsNullOrWhiteSpace(contract.SignedAgreementPath))
            return null;

        var filePath = Path.Combine(_environment.ContentRootPath, contract.SignedAgreementPath);

        if (!File.Exists(filePath))
            return null;

        return (filePath, "application/pdf", Path.GetFileName(filePath));
    }
}

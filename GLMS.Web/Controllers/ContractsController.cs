/*Author: Zenwyn Naicker*/
/*Code Attribution: */
/**************************************************************************************************************
 * Author: Microsoft
 * Date: 2026
 * Title: Get started with ASP.NET Core MVC
 * Type: [Source Code]
 * Available at: <https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-mvc-app/start-mvc?view=aspnetcore-10.0> [Accessed 6 April 2026]
 **************************************************************************************************************/
/**************************************************************************************************************
 * Author: Microsoft
 * Date: 2025
 * Title: Upload files in ASP.NET Core
 * Type: [Source Code]
 * Available at: <https://learn.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads?view=aspnetcore-10.0> [Accessed 6 April 2026]
 **************************************************************************************************************/

using GLMS.Web.Services;
using GLMS.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GLMS.Web.Controllers;

public class ContractsController : Controller
{
    private readonly IGlmsApiClient _apiClient;

    public ContractsController(IGlmsApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate, ContractStatusViewModel? status)
    {
        var contracts = await _apiClient.GetContractsAsync(startDate, endDate, status);

        ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
        ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");
        ViewBag.Status = status;

        return View(contracts);
    }

    public async Task<IActionResult> Details(int id)
    {
        var contract = await _apiClient.GetContractAsync(id);

        if (contract is null)
            return NotFound();

        return View(contract);
    }

    public async Task<IActionResult> Create()
    {
        await PopulateClientsAsync();

        return View(new CreateContractViewModel
        {
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddYears(1)
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateContractViewModel model)
    {
        ValidateSignedAgreement(model);

        if (!ModelState.IsValid)
        {
            await PopulateClientsAsync(model.ClientId);
            return View(model);
        }

        try
        {
            var created = await _apiClient.CreateContractAsync(model);

            await _apiClient.UploadAgreementAsync(created.Id, model.SignedAgreement!);

            return RedirectToAction(nameof(Index));
        }
        catch (ApiClientException ex)
        {
            ModelState.AddModelError("", ex.Message);
            await PopulateClientsAsync(model.ClientId);
            return View(model);
        }
    }

    private void ValidateSignedAgreement(CreateContractViewModel model)
    {
        if (model.SignedAgreement is null)
        {
            ModelState.AddModelError(nameof(model.SignedAgreement), "Please upload a signed PDF agreement.");
            return;
        }

        if (model.SignedAgreement.Length == 0)
        {
            ModelState.AddModelError(nameof(model.SignedAgreement), "Agreement file cannot be empty.");
            return;
        }

        var extension = Path.GetExtension(model.SignedAgreement.FileName);

        if (!extension.Equals(".pdf", StringComparison.OrdinalIgnoreCase))
            ModelState.AddModelError(nameof(model.SignedAgreement), "Only PDF files are allowed.");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, ContractStatusViewModel newStatus)
    {
        var updated = await _apiClient.UpdateContractStatusAsync(id, newStatus);

        if (updated is null)
            return NotFound();

        return RedirectToAction(nameof(Details), new { id });
    }

    public async Task<IActionResult> DownloadAgreement(int id)
    {
        var download = await _apiClient.DownloadAgreementAsync(id);

        if (download is null)
            return NotFound();

        return File(download.Content, download.ContentType, download.FileName);
    }

    private async Task PopulateClientsAsync(int? selectedClientId = null)
    {
        var clients = await _apiClient.GetClientsAsync();
        ViewBag.Clients = new SelectList(clients, nameof(ClientViewModel.Id), nameof(ClientViewModel.Name), selectedClientId);
    }
}

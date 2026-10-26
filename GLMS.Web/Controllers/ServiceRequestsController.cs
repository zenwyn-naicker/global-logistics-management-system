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
 * Author: ExchangeRate-API
 * Date: 2026
 * Title: Pair Conversion Requests
 * Type: [Source Code]
 * Available at: <https://www.exchangerate-api.com/docs/pair-conversion-requests> [Accessed 6 April 2026]
 **************************************************************************************************************/

using GLMS.Web.Services;
using GLMS.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GLMS.Web.Controllers;

public class ServiceRequestsController : Controller
{
    private readonly IGlmsApiClient _apiClient;

    public ServiceRequestsController(IGlmsApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<IActionResult> Create(int contractId)
    {
        ViewBag.ContractId = contractId;
        ViewBag.ActiveStrategy = (await _apiClient.GetCurrencyRateAsync()).Strategy;

        return View(new CreateServiceRequestViewModel
        {
            ContractId = contractId
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateServiceRequestViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.ContractId = model.ContractId;
            ViewBag.ActiveStrategy = (await _apiClient.GetCurrencyRateAsync()).Strategy;
            return View(model);
        }

        try
        {
            await _apiClient.CreateServiceRequestAsync(model);
            return RedirectToAction("Details", "Contracts", new { id = model.ContractId });
        }
        catch (ApiClientException ex)
        {
            ModelState.AddModelError("", ex.Message);
            ViewBag.ContractId = model.ContractId;
            ViewBag.ActiveStrategy = (await _apiClient.GetCurrencyRateAsync()).Strategy;
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> PreviewZar(decimal costInUsd)
    {
        if (costInUsd < 0)
            return BadRequest("Cost cannot be negative.");

        var preview = await _apiClient.PreviewCurrencyAsync(costInUsd);

        return Json(new
        {
            amount = preview.AmountZar,
            formatted = $"R {preview.AmountZar:N2}",
            strategy = preview.Strategy
        });
    }
}

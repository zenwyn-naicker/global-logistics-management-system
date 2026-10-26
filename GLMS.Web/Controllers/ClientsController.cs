/*Author: Zenwyn Naicker*/
/*Code Attribution: */
/**************************************************************************************************************
 * Author: Microsoft
 * Date: 2026
 * Title: Get started with ASP.NET Core MVC
 * Type: [Source Code]
 * Available at: <https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-mvc-app/start-mvc?view=aspnetcore-10.0> [Accessed 6 April 2026]
 **************************************************************************************************************/

using GLMS.Web.Services;
using GLMS.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GLMS.Web.Controllers;

public class ClientsController : Controller
{
    private readonly IGlmsApiClient _apiClient;

    public ClientsController(IGlmsApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<IActionResult> Index()
    {
        var clients = await _apiClient.GetClientsAsync();
        return View(clients);
    }

    public IActionResult Create()
    {
        return View(new CreateClientViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateClientViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            await _apiClient.CreateClientAsync(model);
            return RedirectToAction(nameof(Index));
        }
        catch (ApiClientException ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(model);
        }
    }
}

/*Author: Zenwyn Naicker*/
/*Code Attribution: */
/**************************************************************************************************************
 * Author: Microsoft
 * Date: 2026
 * Title: Create web APIs with ASP.NET Core
 * Type: [Source Code]
 * Available at: <https://learn.microsoft.com/en-us/aspnet/core/web-api/?view=aspnetcore-10.0> [Accessed 18 May 2026]
 **************************************************************************************************************/
/**************************************************************************************************************
 * Author: ExchangeRate-API
 * Date: 2026
 * Title: Pair Conversion Requests
 * Type: [Source Code]
 * Available at: <https://www.exchangerate-api.com/docs/pair-conversion-requests> [Accessed 6 April 2026]
 **************************************************************************************************************/

using GLMS.Api.Dtos;
using GLMS.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GLMS.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/service-requests")]
public class ServiceRequestsController : ControllerBase
{
    private readonly IServiceRequestService _serviceRequests;

    public ServiceRequestsController(IServiceRequestService serviceRequests)
    {
        _serviceRequests = serviceRequests;
    }

    [HttpGet]
    public async Task<ActionResult<List<ServiceRequestDto>>> GetServiceRequests([FromQuery] int? contractId)
    {
        return Ok(await _serviceRequests.GetServiceRequestsAsync(contractId));
    }

    [HttpPost]
    public async Task<ActionResult<ServiceRequestDto>> CreateServiceRequest([FromBody] CreateServiceRequestDto dto)
    {
        try
        {
            return Ok(await _serviceRequests.CreateServiceRequestAsync(dto));
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

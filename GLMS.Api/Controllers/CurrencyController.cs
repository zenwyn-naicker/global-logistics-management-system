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
[Route("api/currency")]
public class CurrencyController : ControllerBase
{
    private readonly ICurrencyService _currency;

    public CurrencyController(ICurrencyService currency)
    {
        _currency = currency;
    }

    [HttpGet("usd-to-zar")]
    public async Task<ActionResult<CurrencyRateDto>> GetUsdToZar()
    {
        return Ok(await _currency.GetUsdToZarRateAsync());
    }

    [HttpGet("preview")]
    public async Task<ActionResult<CurrencyPreviewDto>> Preview([FromQuery] decimal amountUsd = 100)
    {
        if (amountUsd <= 0)
            return BadRequest(new { message = "Amount must be greater than zero." });

        return Ok(await _currency.PreviewUsdToZarAsync(amountUsd));
    }
}

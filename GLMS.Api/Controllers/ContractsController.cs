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
 * Author: Microsoft
 * Date: 2025
 * Title: Upload files in ASP.NET Core
 * Type: [Source Code]
 * Available at: <https://learn.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads?view=aspnetcore-10.0> [Accessed 6 April 2026]
 **************************************************************************************************************/

using GLMS.Api.Dtos;
using GLMS.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GLMS.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/contracts")]
public class ContractsController : ControllerBase
{
    private readonly IAgreementService _agreements;
    private readonly IContractService _contracts;

    public ContractsController(IContractService contracts, IAgreementService agreements)
    {
        _contracts = contracts;
        _agreements = agreements;
    }

    [HttpGet]
    public async Task<ActionResult<List<ContractDto>>> GetContracts([FromQuery] ContractFilterDto filter)
    {
        return Ok(await _contracts.GetContractsAsync(filter));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ContractDto>> GetContract(int id)
    {
        var contract = await _contracts.GetContractAsync(id);

        if (contract is null)
            return NotFound();

        return Ok(contract);
    }

    [HttpPost]
    public async Task<ActionResult<ContractDto>> CreateContract([FromBody] CreateContractDto dto)
    {
        try
        {
            var created = await _contracts.CreateContractAsync(dto);
            return CreatedAtAction(nameof(GetContract), new { id = created.Id }, created);
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPatch("{id:int}/status")]
    public async Task<ActionResult<ContractDto>> UpdateStatus(int id, [FromBody] UpdateContractStatusDto dto)
    {
        var updated = await _contracts.UpdateStatusAsync(id, dto);

        if (updated is null)
            return NotFound();

        return Ok(updated);
    }

    [HttpPost("{id:int}/agreement")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<AgreementUploadResultDto>> UploadAgreement(int id, IFormFile file)
    {
        try
        {
            return Ok(await _agreements.UploadAgreementAsync(id, file));
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

    [HttpGet("{id:int}/agreement")]
    public async Task<IActionResult> GetAgreement(int id)
    {
        var file = await _agreements.GetAgreementFileAsync(id);

        if (file is null)
            return NotFound();

        return PhysicalFile(file.Value.FilePath, file.Value.ContentType, file.Value.DownloadName);
    }
}

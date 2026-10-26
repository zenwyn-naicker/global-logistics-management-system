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
 * Title: Configure JWT bearer authentication in ASP.NET Core
 * Type: [Source Code]
 * Available at: <https://learn.microsoft.com/en-us/aspnet/core/security/authentication/configure-jwt-bearer-authentication?view=aspnetcore-10.0> [Accessed 18 May 2026]
 **************************************************************************************************************/
/**************************************************************************************************************
 * Author: Microsoft
 * Date: 2026
 * Title: Configuration in ASP.NET Core
 * Type: [Source Code]
 * Available at: <https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-10.0> [Accessed 6 April 2026]
 **************************************************************************************************************/

using GLMS.Api.Dtos;
using GLMS.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GLMS.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IJwtTokenService _tokens;

    public AuthController(IConfiguration configuration, IJwtTokenService tokens)
    {
        _configuration = configuration;
        _tokens = tokens;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public ActionResult<LoginResponseDto> Login([FromBody] LoginRequestDto dto)
    {
        var username = _configuration["Auth:Username"];
        var password = _configuration["Auth:Password"];

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            throw new InvalidOperationException("Configure Auth:Username and Auth:Password before using the API.");

        if (!dto.Username.Equals(username, StringComparison.OrdinalIgnoreCase) || dto.Password != password)
            return Unauthorized(new { message = "Invalid username or password." });

        return Ok(_tokens.GenerateToken(dto.Username));
    }
}

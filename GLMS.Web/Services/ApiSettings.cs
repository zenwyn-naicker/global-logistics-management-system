/*Author: Zenwyn Naicker*/
/*Code Attribution: */
/**************************************************************************************************************
 * Author: Microsoft
 * Date: 2026
 * Title: Configuration in ASP.NET Core
 * Type: [Source Code]
 * Available at: <https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-10.0> [Accessed 6 April 2026]
 **************************************************************************************************************/

namespace GLMS.Web.Services;

public class ApiSettings
{
    public string BaseUrl { get; set; } = "https://localhost:7299/";
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

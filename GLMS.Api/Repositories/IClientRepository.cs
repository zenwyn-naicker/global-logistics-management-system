/*Author: Zenwyn Naicker*/
/*Code Attribution: */
/**************************************************************************************************************
 * Author: Microsoft
 * Date: 2022
 * Title: Implementing the Repository and Unit of Work Patterns in an ASP.NET MVC Application (9 of 10)
 * Type: [Source Code]
 * Available at: <https://learn.microsoft.com/en-us/aspnet/mvc/overview/older-versions/getting-started-with-ef-5-using-mvc-4/implementing-the-repository-and-unit-of-work-patterns-in-an-asp-net-mvc-application> [Accessed 18 May 2026]
 **************************************************************************************************************/

using GLMS.Api.Models;

namespace GLMS.Api.Repositories;

public interface IClientRepository
{
    Task<List<Client>> GetAllAsync();
    Task<Client?> GetByIdAsync(int id);
    Task<Client> AddAsync(Client client);
    Task<bool> ExistsAsync(int id);
}

/*Author: Zenwyn Naicker*/
/*Code Attribution: */
/**************************************************************************************************************
 * Author: Microsoft
 * Date: 2022
 * Title: Implementing the Repository and Unit of Work Patterns in an ASP.NET MVC Application (9 of 10)
 * Type: [Source Code]
 * Available at: <https://learn.microsoft.com/en-us/aspnet/mvc/overview/older-versions/getting-started-with-ef-5-using-mvc-4/implementing-the-repository-and-unit-of-work-patterns-in-an-asp-net-mvc-application> [Accessed 18 May 2026]
 **************************************************************************************************************/
/**************************************************************************************************************
 * Author: Microsoft
 * Date: 2023
 * Title: Getting Started - EF Core
 * Type: [Source Code]
 * Available at: <https://learn.microsoft.com/en-us/ef/core/get-started/overview/first-app?tabs=netcore-cli> [Accessed 6 April 2026]
 **************************************************************************************************************/

using GLMS.Api.Data;
using GLMS.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GLMS.Api.Repositories;

public class ClientRepository : IClientRepository
{
    private readonly GlmsDbContext _context;

    public ClientRepository(GlmsDbContext context)
    {
        _context = context;
    }

    public async Task<List<Client>> GetAllAsync()
    {
        return await _context.Clients
            .AsNoTracking()
            .Include(client => client.Contracts)
            .OrderBy(client => client.Name)
            .ToListAsync();
    }

    public async Task<Client?> GetByIdAsync(int id)
    {
        return await _context.Clients
            .Include(client => client.Contracts)
            .FirstOrDefaultAsync(client => client.Id == id);
    }

    public async Task<Client> AddAsync(Client client)
    {
        _context.Clients.Add(client);
        await _context.SaveChangesAsync();
        return client;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Clients.AnyAsync(client => client.Id == id);
    }
}

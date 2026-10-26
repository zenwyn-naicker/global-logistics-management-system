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

public class ContractRepository : IContractRepository
{
    private readonly GlmsDbContext _context;

    public ContractRepository(GlmsDbContext context)
    {
        _context = context;
    }

    public async Task<List<Contract>> GetAllAsync(
        DateTime? startDate = null,
        DateTime? endDate = null,
        ContractStatus? status = null)
    {
        var query = _context.Contracts
            .AsNoTracking()
            .Include(contract => contract.Client)
            .Include(contract => contract.ServiceRequests)
            .AsQueryable();

        if (startDate.HasValue)
            query = query.Where(contract => contract.StartDate >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(contract => contract.EndDate <= endDate.Value);

        if (status.HasValue)
            query = query.Where(contract => contract.Status == status.Value);

        return await query
            .OrderByDescending(contract => contract.StartDate)
            .ToListAsync();
    }

    public async Task<Contract?> GetByIdAsync(int id)
    {
        return await _context.Contracts
            .Include(contract => contract.Client)
            .Include(contract => contract.ServiceRequests)
            .FirstOrDefaultAsync(contract => contract.Id == id);
    }

    public async Task<Contract> AddAsync(Contract contract)
    {
        _context.Contracts.Add(contract);
        await _context.SaveChangesAsync();
        return contract;
    }

    public async Task UpdateAsync(Contract contract)
    {
        _context.Contracts.Update(contract);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Contracts.AnyAsync(contract => contract.Id == id);
    }
}

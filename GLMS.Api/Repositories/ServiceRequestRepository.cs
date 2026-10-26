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

public class ServiceRequestRepository : IServiceRequestRepository
{
    private readonly GlmsDbContext _context;

    public ServiceRequestRepository(GlmsDbContext context)
    {
        _context = context;
    }

    public async Task<List<ServiceRequest>> GetAllAsync()
    {
        return await _context.ServiceRequests
            .AsNoTracking()
            .Include(request => request.Contract)
            .OrderByDescending(request => request.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<ServiceRequest>> GetByContractIdAsync(int contractId)
    {
        return await _context.ServiceRequests
            .AsNoTracking()
            .Where(request => request.ContractId == contractId)
            .OrderByDescending(request => request.CreatedAt)
            .ToListAsync();
    }

    public async Task<ServiceRequest?> GetByIdAsync(int id)
    {
        return await _context.ServiceRequests
            .Include(request => request.Contract)
            .FirstOrDefaultAsync(request => request.Id == id);
    }

    public async Task<ServiceRequest> AddAsync(ServiceRequest serviceRequest)
    {
        _context.ServiceRequests.Add(serviceRequest);
        await _context.SaveChangesAsync();
        return serviceRequest;
    }

    public async Task UpdateAsync(ServiceRequest serviceRequest)
    {
        _context.ServiceRequests.Update(serviceRequest);
        await _context.SaveChangesAsync();
    }
}

/*Author: Zenwyn Naicker*/
/*Code Attribution: */
/**************************************************************************************************************
 * Author: Refactoring.Guru
 * Date: 2026
 * Title: Strategy in C#
 * Type: [Source Code]
 * Available at: <https://refactoring.guru/design-patterns/strategy/csharp/example> [Accessed 6 April 2026]
 **************************************************************************************************************/
/**************************************************************************************************************
 * Author: Microsoft
 * Date: 2022
 * Title: Implementing the Repository and Unit of Work Patterns in an ASP.NET MVC Application (9 of 10)
 * Type: [Source Code]
 * Available at: <https://learn.microsoft.com/en-us/aspnet/mvc/overview/older-versions/getting-started-with-ef-5-using-mvc-4/implementing-the-repository-and-unit-of-work-patterns-in-an-asp-net-mvc-application> [Accessed 18 May 2026]
 **************************************************************************************************************/

using GLMS.Api.Dtos;
using GLMS.Api.Models;
using GLMS.Api.Repositories;

namespace GLMS.Api.Services;

public class ServiceRequestService : IServiceRequestService
{
    private readonly IContractRepository _contracts;
    private readonly IServiceRequestRepository _serviceRequests;
    private readonly ICurrencyService _currency;

    public ServiceRequestService(
        IContractRepository contracts,
        IServiceRequestRepository serviceRequests,
        ICurrencyService currency)
    {
        _contracts = contracts;
        _serviceRequests = serviceRequests;
        _currency = currency;
    }

    public async Task<List<ServiceRequestDto>> GetServiceRequestsAsync(int? contractId = null)
    {
        var requests = contractId.HasValue
            ? await _serviceRequests.GetByContractIdAsync(contractId.Value)
            : await _serviceRequests.GetAllAsync();

        return requests.Select(ToDto).ToList();
    }

    public async Task<ServiceRequestDto> CreateServiceRequestAsync(CreateServiceRequestDto dto)
    {
        var contract = await _contracts.GetByIdAsync(dto.ContractId);

        if (contract is null)
            throw new ArgumentException("Contract not found.");

        if (contract.Status != ContractStatus.Active)
            throw new InvalidOperationException($"Cannot create service request. Contract must be Active, but is {contract.Status}.");

        var costZar = await _currency.ConvertUsdToZarAsync(dto.CostUsd);

        var serviceRequest = new ServiceRequest
        {
            ContractId = dto.ContractId,
            Description = dto.Description,
            Cost = costZar,
            Status = ServiceRequestStatus.Open
        };

        var created = await _serviceRequests.AddAsync(serviceRequest);
        return ToDto(created);
    }

    private static ServiceRequestDto ToDto(ServiceRequest request)
    {
        return new ServiceRequestDto
        {
            Id = request.Id,
            ContractId = request.ContractId,
            Description = request.Description,
            CostZar = request.Cost,
            Status = request.Status,
            CreatedAt = request.CreatedAt
        };
    }
}

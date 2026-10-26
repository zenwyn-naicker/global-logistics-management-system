/*Author: Zenwyn Naicker*/
/*Code Attribution: */
/**************************************************************************************************************
 * Author: Refactoring.Guru
 * Date: 2026
 * Title: Factory Method in C#
 * Type: [Source Code]
 * Available at: <https://refactoring.guru/design-patterns/factory-method/csharp/example> [Accessed 6 April 2026]
 **************************************************************************************************************/
/**************************************************************************************************************
 * Author: Refactoring.Guru
 * Date: 2026
 * Title: Observer in C#
 * Type: [Source Code]
 * Available at: <https://refactoring.guru/design-patterns/observer/csharp/example> [Accessed 6 April 2026]
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
using GLMS.Api.Services.Factories;
using GLMS.Api.Services.Observers;

namespace GLMS.Api.Services;

public class ContractService : IContractService
{
    private readonly IClientRepository _clients;
    private readonly IContractRepository _contracts;
    private readonly ContractFactoryResolver _factoryResolver;
    private readonly ContractStatusSubject _statusSubject;

    public ContractService(
        IClientRepository clients,
        IContractRepository contracts,
        ContractFactoryResolver factoryResolver,
        ContractStatusSubject statusSubject)
    {
        _clients = clients;
        _contracts = contracts;
        _factoryResolver = factoryResolver;
        _statusSubject = statusSubject;
    }

    public async Task<List<ContractDto>> GetContractsAsync(ContractFilterDto filter)
    {
        var contracts = await _contracts.GetAllAsync(filter.StartDate, filter.EndDate, filter.Status);
        return contracts.Select(ToDto).ToList();
    }

    public async Task<ContractDto?> GetContractAsync(int id)
    {
        var contract = await _contracts.GetByIdAsync(id);
        return contract is null ? null : ToDto(contract);
    }

    public async Task<ContractDto> CreateContractAsync(CreateContractDto dto)
    {
        if (!await _clients.ExistsAsync(dto.ClientId))
            throw new ArgumentException($"Client {dto.ClientId} does not exist.");

        if (dto.EndDate < dto.StartDate)
            throw new InvalidOperationException("End date cannot be earlier than start date.");

        var factory = _factoryResolver.Resolve(dto.Status);
        var contract = factory.CreateContract(dto.ClientId, dto.StartDate, dto.EndDate, dto.ServiceLevel);
        contract.SignedAgreementPath = dto.SignedAgreementPath;

        var created = await _contracts.AddAsync(contract);
        var loaded = await _contracts.GetByIdAsync(created.Id);

        return ToDto(loaded ?? created);
    }

    public async Task<ContractDto?> UpdateStatusAsync(int id, UpdateContractStatusDto dto)
    {
        var contract = await _contracts.GetByIdAsync(id);

        if (contract is null)
            return null;

        var oldStatus = contract.Status;
        contract.Status = dto.Status;

        await _contracts.UpdateAsync(contract);

        _statusSubject.NotifyObservers(new ContractStatusChangedEvent
        {
            ContractId = id,
            OldStatus = oldStatus,
            NewStatus = dto.Status
        });

        return ToDto(contract);
    }

    private static ContractDto ToDto(Contract contract)
    {
        return new ContractDto
        {
            Id = contract.Id,
            ClientId = contract.ClientId,
            ClientName = contract.Client?.Name ?? string.Empty,
            StartDate = contract.StartDate,
            EndDate = contract.EndDate,
            Status = contract.Status,
            ServiceLevel = contract.ServiceLevel,
            SignedAgreementPath = contract.SignedAgreementPath,
            ServiceRequestCount = contract.ServiceRequests.Count
        };
    }
}

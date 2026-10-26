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
 * Date: 2022
 * Title: Implementing the Repository and Unit of Work Patterns in an ASP.NET MVC Application (9 of 10)
 * Type: [Source Code]
 * Available at: <https://learn.microsoft.com/en-us/aspnet/mvc/overview/older-versions/getting-started-with-ef-5-using-mvc-4/implementing-the-repository-and-unit-of-work-patterns-in-an-asp-net-mvc-application> [Accessed 18 May 2026]
 **************************************************************************************************************/

using GLMS.Api.Dtos;
using GLMS.Api.Models;
using GLMS.Api.Repositories;

namespace GLMS.Api.Services;

public class ClientService : IClientService
{
    private readonly IClientRepository _clients;

    public ClientService(IClientRepository clients)
    {
        _clients = clients;
    }

    public async Task<List<ClientDto>> GetClientsAsync()
    {
        var clients = await _clients.GetAllAsync();
        return clients.Select(ToDto).ToList();
    }

    public async Task<ClientDto?> GetClientAsync(int id)
    {
        var client = await _clients.GetByIdAsync(id);
        return client is null ? null : ToDto(client);
    }

    public async Task<ClientDto> CreateClientAsync(CreateClientDto dto)
    {
        var client = new Client
        {
            Name = dto.Name,
            ContactDetails = dto.ContactDetails,
            Region = dto.Region
        };

        var created = await _clients.AddAsync(client);
        return ToDto(created);
    }

    private static ClientDto ToDto(Client client)
    {
        return new ClientDto
        {
            Id = client.Id,
            Name = client.Name,
            ContactDetails = client.ContactDetails,
            Region = client.Region,
            ContractCount = client.Contracts.Count
        };
    }
}

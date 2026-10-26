using GLMS.Api.Dtos;

namespace GLMS.Api.Services;

public interface IClientService
{
    Task<List<ClientDto>> GetClientsAsync();
    Task<ClientDto?> GetClientAsync(int id);
    Task<ClientDto> CreateClientAsync(CreateClientDto dto);
}

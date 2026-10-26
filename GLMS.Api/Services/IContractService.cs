using GLMS.Api.Dtos;

namespace GLMS.Api.Services;

public interface IContractService
{
    Task<List<ContractDto>> GetContractsAsync(ContractFilterDto filter);
    Task<ContractDto?> GetContractAsync(int id);
    Task<ContractDto> CreateContractAsync(CreateContractDto dto);
    Task<ContractDto?> UpdateStatusAsync(int id, UpdateContractStatusDto dto);
}

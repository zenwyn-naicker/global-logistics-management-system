using GLMS.Api.Dtos;

namespace GLMS.Api.Services;

public interface IServiceRequestService
{
    Task<List<ServiceRequestDto>> GetServiceRequestsAsync(int? contractId = null);
    Task<ServiceRequestDto> CreateServiceRequestAsync(CreateServiceRequestDto dto);
}

using GLMS.Api.Dtos;

namespace GLMS.Api.Services;

public interface IJwtTokenService
{
    LoginResponseDto GenerateToken(string username);
}

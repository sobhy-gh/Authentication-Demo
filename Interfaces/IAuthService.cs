using AuthenticationDemo.Dtos;

namespace AuthenticationDemo.Interfaces
{
	public interface IAuthService
	{
		 Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto requestDto);
		 Task<AuthResponse> LoginAsync(LoginRequest requestDto);
		Task LogoutAsync(string refreshToken, int userId);
	}
}

using AuthenticationDemo.Dtos.UserDtos;

namespace AuthenticationDemo.Interfaces
{
	public interface IUserService
	{
		Task<List<UserResponseDto>> GetAllAsync();
		Task<UserResponseDto?> GetByIdAsync(int id);
		Task<UserResponseDto> CreateAsync(CreateUserDto request); 
		Task<UserResponseDto?> UpdateAsync(int id , UpdateUserDto request);
		Task<bool> DeleteAsync(int id);
	}
}

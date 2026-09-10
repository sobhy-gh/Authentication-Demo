using AuthenticationDemo.Data;
using AuthenticationDemo.Dtos.UserDtos;
using AuthenticationDemo.Entities;
using AuthenticationDemo.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

namespace AuthenticationDemo.Services
{
	public class UserService : IUserService 
	{

		private readonly AppDbContext _context;
		private readonly IPasswordHasher<User> _passwordHasher;

		public UserService(AppDbContext context , IPasswordHasher<User> passwordHasher)
		{
			_context = context;
			_passwordHasher = passwordHasher;
		}

		public async Task<List<UserResponseDto>> GetAllAsync()
		{

			return await _context.Users
			.Select(u => new UserResponseDto
			{
				Id = u.Id,
				Username = u.Username,
				Email = u.Email,
				IsActive = u.IsActive
			})
			.ToListAsync();
		}

		public async Task<UserResponseDto?> GetByIdAsync(int id)
		{

			 return  await _context.Users
			   .Where(u => u.Id == id)
			   .Select(u => new UserResponseDto
			   {
				   Id = u.Id,
				   Username = u.Username,
				   Email = u.Email,
				   IsActive = u.IsActive
			   })
		   
			 .FirstOrDefaultAsync(); 

		}

		public async Task<UserResponseDto> CreateAsync(CreateUserDto request) 
		{

			var user = new User
			{
				Username = request.Username,
				Email = request.Email,
				IsActive = true,
				CreatedAt = DateTime.UtcNow
			};

			user.HashedPassword = _passwordHasher.HashPassword(user, request.Password);

			_context.Users.Add(user);

			await _context.SaveChangesAsync();

			return new UserResponseDto
			{

				Id = user.Id,
				Username = user.Username,
				Email = user.Email,
				IsActive = user.IsActive
			}; 

		}

		public async Task<UserResponseDto?> UpdateAsync(int id , UpdateUserDto request)
		{

			var userExistance = await _context.Users
				  .FirstOrDefaultAsync(u => u.Id == id);

			if (userExistance is null)
				return null;


			userExistance.Username = request.Username;
			userExistance.Email = request.Email;

			await _context.SaveChangesAsync();


			return new UserResponseDto
			{
				Id = userExistance.Id,
				Username = userExistance.Username,
				Email = userExistance.Email,
				IsActive = userExistance.IsActive
			};

		}

		public async Task<bool> DeleteAsync(int id)
		{

			var userExistance = await _context.Users
				 .FirstOrDefaultAsync(u => u.Id == id);

			if (userExistance is null)
				return false;

			_context.Users.Remove(userExistance);

			await _context.SaveChangesAsync();

			return true; 
		}
	}
}

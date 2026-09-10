using AuthenticationDemo.Data;
using AuthenticationDemo.Dtos;
using AuthenticationDemo.Entities;
using AuthenticationDemo.Interfaces;
using AuthenticationDemo.Validation;
using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace AuthenticationDemo.Services
{
	public class AuthService : IAuthService 
	{
		private readonly AppDbContext _context;

		private readonly JwtTokenService _jwtTokenService;
		private readonly IPasswordHasher<User> _passwordHasher;
		private readonly RefreshToeknServices _refreshToeknService;
		public AuthService(AppDbContext context, IPasswordHasher<User> passwordHasher , JwtTokenService jwtTokenService , RefreshToeknServices refreshToeknService)
		{
			  _context = context; 
			  _passwordHasher = passwordHasher;	 
			  _jwtTokenService = jwtTokenService;
			  _refreshToeknService = refreshToeknService;
		}


		public async Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto requestDto)
		{
			// Normalize input
			var username = requestDto.Username.Trim();
			var email = requestDto.Email.Trim().ToLowerInvariant();

			// Validate password
			bool isValid = Regex.IsMatch(
				requestDto.password,
				ValidationConstants.PasswordRegex);

			if (!isValid)
			{
				throw new InvalidOperationException(
					"Wrong password, please try again.");
			}

			// Check email
			var emailExists = await _context.Users
				.AnyAsync(u => u.Email == email);

			if (emailExists)
			{
				throw new InvalidOperationException(
					"Email is already registered.");
			}

			// Check username
			var usernameExists = await _context.Users
				.AnyAsync(u => u.Username == username);

			if (usernameExists)
			{
				throw new InvalidOperationException(
					"This username is already taken, try another one!");
			}

			// Get default role
			var defaultRole = await _context.Roles
				.SingleOrDefaultAsync(r => r.Name == "User");

			if (defaultRole is null)
			{
				throw new InvalidOperationException(
					"Default role is not configured.");
			}

			// Create user
			var user = new User
			{
				Username = username,
				Email = email,
				CreatedAt = DateTime.UtcNow
			};

			user.HashedPassword =
				_passwordHasher.HashPassword(
					user,
					requestDto.password);

			// Assign default role
			user.UserRoles.Add(new UserRole
			{
				Role = defaultRole
			});

			_context.Users.Add(user);

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateException ex)
			{
				throw new InvalidOperationException(
					"Username or Email is already taken.",
					ex);
			}

			return new RegisterResponseDto
			{
				Id = user.Id,
				Username = user.Username,
				Email = user.Email
			};
		}
		public async Task<AuthResponse> LoginAsync(LoginRequest requestDto)
		{
		   // Validate user inputs 

		   if(string.IsNullOrEmpty(requestDto.Email) || string.IsNullOrEmpty(requestDto.Password))
		   {

				throw new ArgumentException("Required Username and Password ");
		   }

			var checkUserExistance = await _context.Users
						 .Include(u => u.UserRoles)
						 .ThenInclude(ur => ur.Role)
					.FirstOrDefaultAsync(u => u.Email == requestDto.Email); 

			if(checkUserExistance is null)
			{

				throw new UnauthorizedAccessException("Invalid Email or Password !");
			}


			if(!checkUserExistance.IsActive)
			{
				throw new UnauthorizedAccessException("Invalid Email or Password !");
			}

			// Checking password 

			var passwordResult = _passwordHasher.VerifyHashedPassword(checkUserExistance, checkUserExistance.HashedPassword, requestDto.Password);

			if(passwordResult == PasswordVerificationResult.Failed) 
			{

				throw new UnauthorizedAccessException("Invalid Email or Password !");
			}

			// Generate Tokens 

			var (accessToken, refreshToken) = _jwtTokenService.GenerateToken(checkUserExistance);

			// Saving user refresh Token in the database 

			await _refreshToeknService.SaveRefreshTokenAsync(checkUserExistance.Id, refreshToken); 

			return new AuthResponse(accessToken, refreshToken);

		}

		public async Task LogoutAsync(string refreshToken, int userId)
		{
			var token = await _context.RefreshTokens
				 .FirstOrDefaultAsync(x =>
					x.Token == refreshToken && x.UserId == userId
				 );

			if (token is null)
				return; 

			token.RevokedAt = DateTime.UtcNow;  // I make this token invalid 

			await _context.SaveChangesAsync();
		}
	}
}

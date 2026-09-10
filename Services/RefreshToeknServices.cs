using AuthenticationDemo.Data;
using AuthenticationDemo.Dtos;
using AuthenticationDemo.Entities;
using Azure.Core;
using Azure.Core.GeoJson;
using Azure.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationDemo.Services
{
	public class RefreshToeknServices
	{
		private readonly AppDbContext _context;
		private readonly JwtTokenService _jwtTokenService;

		public RefreshToeknServices(AppDbContext context, JwtTokenService jwtTokenService)
		{
			_context = context;
			_jwtTokenService = jwtTokenService;
		}


		public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
		{
			return await _context.RefreshTokens
					.FirstOrDefaultAsync(r => r.Token == token);
		}

		public async Task SaveRefreshTokenAsync(int userId, string refreshToken)
		{

			var refreshTokenRecord = new RefreshToken
			{
				UserId = userId,
				Token = refreshToken,
				ExpiresAt = DateTime.UtcNow.AddDays(30)
			};

			_context.RefreshTokens.Add(refreshTokenRecord);

			await _context.SaveChangesAsync();
		}



		public async Task<AuthResponse> RefreshTokenAsync(string refreshToken)
		{
			var now = DateTime.UtcNow;

			// 1. Find refresh token
			var storedToken = await _context.RefreshTokens
				.Include(r => r.User)
					.ThenInclude(u => u.UserRoles)
						.ThenInclude(ur => ur.Role)
				.FirstOrDefaultAsync(r => r.Token == refreshToken);

			// 2. Check token existence
			if (storedToken is null)
			{
				throw new AuthenticationFailedException(
					"Invalid refresh token.");
			}

			// 3. Check if token has expired
			if (storedToken.ExpiresAt <= now)
			{
				throw new AuthenticationFailedException(
					"Refresh token has expired.");
			}

			// 4. Check if token has already been revoked
			if (storedToken.RevokedAt != null)
			{
				// Revoke all active refresh tokens for this user
				await _context.RefreshTokens
					.Where(r =>
						r.UserId == storedToken.UserId &&
						r.RevokedAt == null)
					.ExecuteUpdateAsync(s =>
						s.SetProperty(r => r.RevokedAt, now));

				throw new AuthenticationFailedException(
					"Invalid refresh token.");
			}

			// 5. Check if user is active
			if (!storedToken.User.IsActive)
			{
				throw new AuthenticationFailedException(
					"Invalid refresh token.");
			}

			// 6. Revoke the current refresh token
			var affected = await _context.RefreshTokens
				.Where(r =>
					r.Id == storedToken.Id &&
					r.RevokedAt == null)
				.ExecuteUpdateAsync(s =>
					s.SetProperty(r => r.RevokedAt, now));

			// 7. Protect against concurrent refresh requests
			if (affected == 0)
			{
				throw new AuthenticationFailedException(
					"Invalid refresh token.");
			}

			// 8. Generate new Access Token + Refresh Token
			var (accessToken, newRefreshToken) =
				_jwtTokenService.GenerateToken(storedToken.User);

			// 9. Save new refresh token
			await SaveRefreshTokenAsync(
				storedToken.UserId,
				newRefreshToken);

			// 10. Return new tokens
			return new AuthResponse(
				accessToken,
				newRefreshToken);
		}
	}
}

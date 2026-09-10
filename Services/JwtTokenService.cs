using AuthenticationDemo.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace AuthenticationDemo.Services
{
	public class JwtTokenService
	{
		private readonly IConfiguration _configuration;

		public JwtTokenService(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public (string AccessToken, string RefreshToken) GenerateToken(User user)
		{
			var jwtSettings = _configuration.GetSection("JwtSettings");

			var issuer = jwtSettings["Issuer"]
				?? throw new InvalidOperationException("JWT Issuer is not configured.");

			var audience = jwtSettings["Audience"]
				?? throw new InvalidOperationException("JWT Audience is not configured.");

			if (!double.TryParse(jwtSettings["ExpiryMinutes"], out var expiryMinutes))
			{
				throw new InvalidOperationException("JWT ExpiryMinutes is not configured correctly.");
			}

			var privateKeyPath = Path.Combine(AppContext.BaseDirectory, "keys", "private.pem");

			if (!File.Exists(privateKeyPath))
			{
				throw new FileNotFoundException("JWT private key was not found.", privateKeyPath);
			}

			var privateKey = File.ReadAllText(privateKeyPath);

			using var rsa = RSA.Create();
			rsa.ImportFromPem(privateKey);

			// تصحيح: تصدير الـ Parameters لإنشاء Key مستقر لا يتأثر بـ Dispose الخاص بـ rsa
			var rsaParameters = rsa.ExportParameters(true);
			var securityKey = new RsaSecurityKey(rsaParameters);

			var signingCredentials = new SigningCredentials(
				securityKey,
				SecurityAlgorithms.RsaSha256);

			var jti = Guid.NewGuid().ToString();

			var claims = new List<Claim>
			{
				new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
				new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // يُفضل إضافتها لتوافق .NET
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
				new Claim(ClaimTypes.Name, user.Username),
				new Claim(JwtRegisteredClaimNames.Jti, jti)
			};

			if (user.UserRoles != null)
			{
				foreach (var userRole in user.UserRoles)
				{
					claims.Add(new Claim(ClaimTypes.Role, userRole.Role.Name));
				}
			}

			var accessToken = new JwtSecurityToken(
				issuer: issuer,
				audience: audience,
				claims: claims,
				expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
				signingCredentials: signingCredentials
			);

			var refreshToken = GenerateRefreshToken();
			var accessTokenString = new JwtSecurityTokenHandler().WriteToken(accessToken);

			return (accessTokenString, refreshToken);
		}

		private static string GenerateRefreshToken()
		{
			var randomBytes = RandomNumberGenerator.GetBytes(64);
			return Convert.ToBase64String(randomBytes);
		}
	}
}
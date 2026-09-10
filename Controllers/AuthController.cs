using AuthenticationDemo.Dtos;
using AuthenticationDemo.Interfaces;
using AuthenticationDemo.Services;
using Azure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace AuthenticationDemo.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly IAuthService _authService;
		private readonly RefreshToeknServices _refreshToeknService;
		private readonly ILogger<AuthController> _logger;


		public AuthController(IAuthService authService, ILogger<AuthController> logger , RefreshToeknServices refreshToeknService)
		{
			_authService = authService;
			_logger = logger;
			_refreshToeknService = refreshToeknService;
		}

		[HttpPost("register")]
		[AllowAnonymous]
		[EnableRateLimiting("fixed")]
		public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
		{
			try
			{
				var result = await _authService.RegisterAsync(request);
				return Ok(result);
			}
			catch (InvalidOperationException ex)
			{
				return Conflict(new { message = ex.Message });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred during user registration");
				return StatusCode(500, new { message = "An error occurred while processing your request." });
			}
		}

		[HttpPost("login")]
		[AllowAnonymous]
		[EnableRateLimiting("fixed")]

		public async Task<IActionResult> Login([FromBody] LoginRequest request) 
		{

		   try
		   {

				var response = await _authService.LoginAsync(request);

				return Ok(response);
		   }
		   catch(ArgumentException ex) 
		   {

				return BadRequest(new
				{

					message = ex.Message
				});
		   }

		   catch(UnauthorizedAccessException ex)
		   {

				return Unauthorized(new
				{

					message = ex.Message
				});
		   }

		}

		[HttpPost("refresh-token")]
		[AllowAnonymous]
		public async Task<IActionResult> RefreshToken(RefreshTokenDto request)
		{

		    try 
			{

				var result = await _refreshToeknService.RefreshTokenAsync(request.RefreshToken);

				return Ok(result);
			}

			catch(AuthenticationFailedException)
			{

				return Unauthorized(new
				{
					message = "Invalid refresh token."
				});
			}
		}

		[HttpPost("logout")]
		[Authorize]
		public async Task<IActionResult> Logout(LogoutDto request)
		{

			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			if (userId is null)

				return Unauthorized();

			await _authService.LogoutAsync(request.RefreshToken, int.Parse(userId));

			return NoContent();

		}

 	}



}
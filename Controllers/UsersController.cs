using AuthenticationDemo.Dtos.UserDtos;
using AuthenticationDemo.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationDemo.Controllers
{
	[Route("api/[controller]")]
	[ApiController]

	[Authorize]
	public class UsersController : ControllerBase
	{
		private readonly IUserService _userService;

		 public UsersController(IUserService userService)
		 {
             _userService = userService;
		 }


		 [HttpGet]
		 public async Task<IActionResult> GetAll()
		 {

			var users = await _userService.GetAllAsync();

			return Ok(users);
		 }

		 [HttpGet("{id:int}")]
   		 [Authorize(Roles = "Admin")]
		public async Task<IActionResult> GetById(int id)
		 {

			var user = await _userService.GetByIdAsync(id);

			if (user is null)
				return NotFound(); 

		    return Ok(user); 
		 }

		 [HttpPost]
		 [Authorize(Roles = "Admin")]
		public async Task<IActionResult> Create(CreateUserDto request)
		 {

			var user = await _userService.CreateAsync(request);

			return CreatedAtAction(
				  nameof(GetById),
				  new {id = user.Id},
				  user);
		 }

		[HttpPut("{id:int}")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Update(
	   int id,
	   UpdateUserDto request)
		{
			var user = await _userService.UpdateAsync(id, request);

			if (user is null)
				return NotFound();

			return Ok(user);
		}

		[HttpDelete("{id:int}")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Delete(int id)
		{
			var deleted = await _userService.DeleteAsync(id);

			if (!deleted)
				return NotFound();

			return NoContent();
		}

	}
}

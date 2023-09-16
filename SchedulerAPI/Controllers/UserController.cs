using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchedulerAPI.Data.Models;
using SchedulerAPI.DTOs;
using SchedulerAPI.Repository;
namespace SchedulerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class UserController : ControllerBase
    {
        private readonly IRepository<ApplicationUser> _userRepository;

        public UserController(IRepository<ApplicationUser> userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userRepository.GetAllAsync();
            return Ok(users.ToDtoList());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user.ToDto());
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(ApplicationUserDTO userDTO)
        {
            var user = userDTO.ToModel();
            await _userRepository.AddAsync(user);
            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, userDTO);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, ApplicationUser user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            try
            {
                await _userRepository.UpdateAsync(user);
            }
            catch (Exception)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                await _userRepository.DeleteAsync(id);
            }
            catch (Exception)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}

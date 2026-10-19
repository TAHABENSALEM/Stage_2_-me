using Microsoft.AspNetCore.Mvc;
using GestionCongeFrais.DTOs;
using GestionCongeFrais.Services;
using Microsoft.AspNetCore.Authorization;

namespace GestionCongeFrais.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }
        [Authorize(Roles = "Admin")]

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _userService.GetAllAsync());
        }
        [Authorize(Roles = "Admin")]

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }
        [Authorize(Roles = "Admin")]

        [HttpPost]
        public async Task<IActionResult> Create(UserDTO dto)
        {
            try
            {
                var created = await _userService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [Authorize(Roles = "Admin")]

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UserDTO dto)
        {
            var success = await _userService.UpdateAsync(id, dto);
            return success ? NoContent() : NotFound();
        }
        [Authorize(Roles = "Admin")]

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _userService.DeleteAsync(id);
            return success ? NoContent() : NotFound();
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("organigram")]
        public async Task<IActionResult> GetOrganigram()
        {
            var organigram = await _userService.GetOrganigramAsync();
            return Ok(organigram);
        }
    }
}

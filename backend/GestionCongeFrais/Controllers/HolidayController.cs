using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using GestionCongeFrais.DTOs;
using GestionCongeFrais.Services;


namespace GestionCongeFrais.Controllers;


[ApiController]
[Route("api/[controller]")]
[Authorize]
public class HolidayController : ControllerBase
{
    private readonly IHolidayService _service;

    public HolidayController(IHolidayService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var holidays = await _service.GetAllAsync();
        return Ok(holidays);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var holiday = await _service.GetByIdAsync(id);
        if (holiday == null) return NotFound();
        return Ok(holiday);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] HolidayDTO dto)
    {
        var newHoliday = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(Get), new { id = newHoliday.Id }, newHoliday);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] HolidayDTO dto)
    {
        var updated = await _service.UpdateAsync(id, dto);
        if (updated == null) return NotFound();
        return Ok(updated);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);
        if (!result) return NotFound();
        return NoContent();
    }
}

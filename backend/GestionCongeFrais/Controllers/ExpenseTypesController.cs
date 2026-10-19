using GestionCongeFrais.DTOs;
using GestionCongeFrais.Models;
using GestionCongeFrais.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ExpenseTypesController : ControllerBase
{
    private readonly IExpenseTypeService _service;

    public ExpenseTypesController(IExpenseTypeService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<ExpenseType>>> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ExpenseType>> Get(int id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<ExpenseType>> Create(ExpenseTypeDTO dto)
    {
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, ExpenseTypeDTO dto)
    {
        var updated = await _service.UpdateAsync(id, dto);
        if (!updated) return NotFound();
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}

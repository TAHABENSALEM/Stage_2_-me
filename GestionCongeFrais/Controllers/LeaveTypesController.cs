using GestionCongeFrais.DTOs;
using GestionCongeFrais.Models;
using GestionCongeFrais.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class LeaveTypesController : ControllerBase
{
    private readonly ILeaveTypeService _leaveTypeService;

    public LeaveTypesController(ILeaveTypeService leaveTypeService)
    {
        _leaveTypeService = leaveTypeService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LeaveType>>> GetLeaveTypes()
    {
        var leaveTypes = await _leaveTypeService.GetAllAsync();
        return Ok(leaveTypes);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<LeaveType>> GetLeaveType(int id)
    {
        var leaveType = await _leaveTypeService.GetByIdAsync(id);
        if (leaveType == null) return NotFound();
        return Ok(leaveType);
    }

    [HttpPost]
    public async Task<ActionResult<LeaveType>> CreateLeaveType(LeaveTypeDTO dto)
    {
        var created = await _leaveTypeService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetLeaveType), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateLeaveType(int id, LeaveTypeDTO dto)
    {
        var success = await _leaveTypeService.UpdateAsync(id, dto);
        if (!success) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLeaveType(int id)
    {
        var success = await _leaveTypeService.DeleteAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }
}

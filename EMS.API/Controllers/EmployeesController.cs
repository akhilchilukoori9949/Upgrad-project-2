using EMS.API.DTOs;
using EMS.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EMS.API.Controllers;

/// <summary>
/// Exposes CRUD, pagination, and dashboard endpoints for employee management.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Viewer")]
public class EmployeesController(EmployeeService employeeService) : ControllerBase
{
    /// <summary>
    /// Returns a paged, filtered, and sorted employee list.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<EmployeeResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<EmployeeResponseDto>>> GetAll([FromQuery] EmployeeQueryParams queryParams, CancellationToken cancellationToken)
    {
        var result = await employeeService.GetAllAsync(queryParams, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Returns a single employee by id.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(EmployeeResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EmployeeResponseDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var employee = await employeeService.GetByIdAsync(id, cancellationToken);
        if (employee is null)
        {
            return NotFound(new { message = "Employee not found." });
        }

        return Ok(employee);
    }

    /// <summary>
    /// Returns the dashboard summary payload.
    /// </summary>
    [HttpGet("dashboard")]
    [ProducesResponseType(typeof(DashboardSummaryDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<DashboardSummaryDto>> GetDashboard(CancellationToken cancellationToken)
    {
        var dashboard = await employeeService.GetDashboardAsync(cancellationToken);
        return Ok(dashboard);
    }

    /// <summary>
    /// Creates a new employee record.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(EmployeeResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] EmployeeRequestDto request, CancellationToken cancellationToken)
    {
        var result = await employeeService.CreateAsync(request, cancellationToken);
        return ToActionResult(result, createdAction: nameof(GetById));
    }

    /// <summary>
    /// Updates an existing employee record.
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(EmployeeResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(int id, [FromBody] EmployeeRequestDto request, CancellationToken cancellationToken)
    {
        var result = await employeeService.UpdateAsync(id, request, cancellationToken);
        return ToActionResult(result);
    }

    /// <summary>
    /// Deletes an employee record.
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await employeeService.DeleteAsync(id, cancellationToken);
        if (!result.Success && string.Equals(result.ErrorCode, "NotFound", StringComparison.OrdinalIgnoreCase))
        {
            return NotFound(new { message = result.Message });
        }

        return Ok(new { success = true, message = result.Message });
    }

    private IActionResult ToActionResult(ServiceResult<EmployeeResponseDto> result, string? createdAction = null)
    {
        if (!result.Success)
        {
            return result.ErrorCode switch
            {
                "Conflict" => Conflict(new { message = result.Message, errors = result.Errors }),
                "NotFound" => NotFound(new { message = result.Message }),
                _ => BadRequest(new { message = result.Message, errors = result.Errors }),
            };
        }

        if (!string.IsNullOrWhiteSpace(createdAction) && result.Data is not null)
        {
            return CreatedAtAction(createdAction, new { id = result.Data.Id }, result.Data);
        }

        return Ok(result.Data);
    }
}

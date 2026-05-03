using System.ComponentModel.DataAnnotations;

namespace EMS.API.DTOs;

/// <summary>
/// Request payload used to create or update an employee.
/// </summary>
public class EmployeeRequestDto
{
    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be exactly 10 digits.")]
    public string Phone { get; set; } = string.Empty;

    [Required]
    [RegularExpression("^(Engineering|Marketing|HR|Finance|Operations)$", ErrorMessage = "Department is invalid.")]
    public string Department { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Designation { get; set; } = string.Empty;

    [Range(typeof(decimal), "0.01", "9999999999999999", ErrorMessage = "Salary must be positive.")]
    public decimal Salary { get; set; }

    [Required]
    public DateTime JoinDate { get; set; }

    [Required]
    [RegularExpression("^(Active|Inactive)$", ErrorMessage = "Status is invalid.")]
    public string Status { get; set; } = string.Empty;
}

/// <summary>
/// Response payload sent to the frontend for employee records.
/// </summary>
public class EmployeeResponseDto
{
    public int Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string Department { get; set; } = string.Empty;

    public string Designation { get; set; } = string.Empty;

    public decimal Salary { get; set; }

    public DateTime JoinDate { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Query string parameters for the employee list endpoint.
/// </summary>
public class EmployeeQueryParams
{
    public string? Search { get; set; }

    public string? Department { get; set; }

    public string? Status { get; set; }

    public string SortBy { get; set; } = "name";

    public string SortDir { get; set; } = "asc";

    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;

    [Range(1, 100)]
    public int PageSize { get; set; } = 10;
}

/// <summary>
/// Generic paged result wrapper returned by the employees list API.
/// </summary>
public class PagedResult<T>
{
    public IReadOnlyList<T> Data { get; set; } = Array.Empty<T>();

    public int TotalCount { get; set; }

    public int Page { get; set; }

    public int PageSize { get; set; }

    public int TotalPages { get; set; }

    public bool HasNextPage { get; set; }

    public bool HasPrevPage { get; set; }
}

/// <summary>
/// Dashboard payload returned in a single API call.
/// </summary>
public class DashboardSummaryDto
{
    public int TotalEmployees { get; set; }

    public int ActiveEmployees { get; set; }

    public int InactiveEmployees { get; set; }

    public int TotalDepartments { get; set; }

    public IReadOnlyList<DepartmentBreakdownDto> DepartmentBreakdown { get; set; } = Array.Empty<DepartmentBreakdownDto>();

    public IReadOnlyList<EmployeeResponseDto> RecentEmployees { get; set; } = Array.Empty<EmployeeResponseDto>();
}

/// <summary>
/// Represents one department's contribution to the workforce mix.
/// </summary>
public class DepartmentBreakdownDto
{
    public string Department { get; set; } = string.Empty;

    public int Count { get; set; }

    public decimal Percentage { get; set; }
}

/// <summary>
/// Registration request payload.
/// </summary>
public class RegisterRequestDto
{
    [Required]
    [StringLength(100)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;

    public string? Role { get; set; }
}

/// <summary>
/// Login request payload.
/// </summary>
public class LoginRequestDto
{
    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// Authentication response payload.
/// </summary>
public class AuthResponseDto
{
    public bool Success { get; set; }

    public string Message { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public string Token { get; set; } = string.Empty;
}

/// <summary>
/// Generic service result used to keep controller actions thin.
/// </summary>
public class ServiceResult<T>
{
    public bool Success { get; init; }

    public string? ErrorCode { get; init; }

    public string Message { get; init; } = string.Empty;

    public T? Data { get; init; }

    public Dictionary<string, string[]>? Errors { get; init; }

    public static ServiceResult<T> Ok(T data, string message = "") =>
        new()
        {
            Success = true,
            Data = data,
            Message = message,
        };

    public static ServiceResult<T> Fail(string errorCode, string message, Dictionary<string, string[]>? errors = null) =>
        new()
        {
            Success = false,
            ErrorCode = errorCode,
            Message = message,
            Errors = errors,
        };
}

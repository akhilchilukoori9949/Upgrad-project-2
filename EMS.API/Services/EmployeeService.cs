using EMS.API.DTOs;
using EMS.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EMS.API.Services;

/// <summary>
/// Contains all employee business logic including filtering, sorting, pagination, and CRUD validation.
/// </summary>
public class EmployeeService(IEmployeeRepository repository)
{
    /// <summary>
    /// Returns a server-filtered and server-paginated employee list.
    /// </summary>
    public async Task<PagedResult<EmployeeResponseDto>> GetAllAsync(EmployeeQueryParams queryParams, CancellationToken cancellationToken = default)
    {
        var normalized = NormalizeQuery(queryParams);
        var query = repository.Query().AsNoTracking();

        if (!string.IsNullOrWhiteSpace(normalized.Search))
        {
            var term = normalized.Search.Trim();
            var pattern = $"%{term}%";

            query = query.Where(e =>
                EF.Functions.Like(e.FirstName + " " + e.LastName, pattern) ||
                EF.Functions.Like(e.Email, pattern));
        }

        if (!string.IsNullOrWhiteSpace(normalized.Department))
        {
            query = query.Where(e => e.Department == normalized.Department);
        }

        if (!string.IsNullOrWhiteSpace(normalized.Status))
        {
            query = query.Where(e => e.Status == normalized.Status);
        }

        query = ApplySorting(query, normalized.SortBy, normalized.SortDir);

        var totalCount = await repository.CountAsync(query);
        var totalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)normalized.PageSize);
        var page = totalPages == 0 ? 1 : Math.Min(normalized.Page, totalPages);
        var skip = (page - 1) * normalized.PageSize;

        var pageItems = await repository.ToListAsync(
            query
                .Skip(skip)
                .Take(normalized.PageSize)
                .Select(MapToResponseProjection));

        return new PagedResult<EmployeeResponseDto>
        {
            Data = pageItems,
            TotalCount = totalCount,
            Page = page,
            PageSize = normalized.PageSize,
            TotalPages = totalPages,
            HasNextPage = totalPages > 0 && page < totalPages,
            HasPrevPage = totalPages > 0 && page > 1,
        };
    }

    /// <summary>
    /// Returns a single employee by identifier.
    /// </summary>
    public async Task<EmployeeResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var employee = await repository.GetByIdAsync(id);
        return employee is null ? null : MapToResponse(employee);
    }

    /// <summary>
    /// Creates a new employee after server-side validation.
    /// </summary>
    public async Task<ServiceResult<EmployeeResponseDto>> CreateAsync(EmployeeRequestDto request, CancellationToken cancellationToken = default)
    {
        if (await repository.EmailExistsAsync(request.Email))
        {
            return ServiceResult<EmployeeResponseDto>.Fail(
                "Conflict",
                "This email address is already used by another employee.",
                new Dictionary<string, string[]>
                {
                    ["email"] = ["This email address is already used by another employee."],
                });
        }

        var employee = new Employee
        {
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = request.Email.Trim(),
            Phone = request.Phone.Trim(),
            Department = request.Department.Trim(),
            Designation = request.Designation.Trim(),
            Salary = request.Salary,
            JoinDate = NormalizeUtc(request.JoinDate),
            Status = request.Status.Trim(),
        };

        await repository.AddAsync(employee);
        await repository.SaveChangesAsync(cancellationToken);

        return ServiceResult<EmployeeResponseDto>.Ok(MapToResponse(employee), "Employee created successfully.");
    }

    /// <summary>
    /// Updates an existing employee if it exists and the email remains unique.
    /// </summary>
    public async Task<ServiceResult<EmployeeResponseDto>> UpdateAsync(int id, EmployeeRequestDto request, CancellationToken cancellationToken = default)
    {
        var employee = await repository.GetTrackedByIdAsync(id);
        if (employee is null)
        {
            return ServiceResult<EmployeeResponseDto>.Fail("NotFound", "Employee not found.");
        }

        if (await repository.EmailExistsAsync(request.Email, id))
        {
            return ServiceResult<EmployeeResponseDto>.Fail(
                "Conflict",
                "This email address is already used by another employee.",
                new Dictionary<string, string[]>
                {
                    ["email"] = ["This email address is already used by another employee."],
                });
        }

        employee.FirstName = request.FirstName.Trim();
        employee.LastName = request.LastName.Trim();
        employee.Email = request.Email.Trim();
        employee.Phone = request.Phone.Trim();
        employee.Department = request.Department.Trim();
        employee.Designation = request.Designation.Trim();
        employee.Salary = request.Salary;
        employee.JoinDate = NormalizeUtc(request.JoinDate);
        employee.Status = request.Status.Trim();

        repository.Update(employee);
        await repository.SaveChangesAsync(cancellationToken);

        return ServiceResult<EmployeeResponseDto>.Ok(MapToResponse(employee), "Employee updated successfully.");
    }

    /// <summary>
    /// Deletes an employee if it exists.
    /// </summary>
    public async Task<ServiceResult<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var employee = await repository.GetTrackedByIdAsync(id);
        if (employee is null)
        {
            return ServiceResult<bool>.Fail("NotFound", "Employee not found.");
        }

        repository.Remove(employee);
        await repository.SaveChangesAsync(cancellationToken);

        return ServiceResult<bool>.Ok(true, "Employee deleted successfully.");
    }

    /// <summary>
    /// Computes dashboard summary metrics and recent employees in a single service call.
    /// </summary>
    public async Task<DashboardSummaryDto> GetDashboardAsync(CancellationToken cancellationToken = default)
    {
        var query = repository.Query().AsNoTracking();
        var totalEmployees = await repository.CountAsync(query);
        var activeEmployees = await repository.CountAsync(query.Where(e => e.Status == "Active"));
        var inactiveEmployees = await repository.CountAsync(query.Where(e => e.Status == "Inactive"));

        var departments = await repository.ToListAsync(
            query
                .GroupBy(e => e.Department)
                .OrderBy(g => g.Key)
                .Select(g => new DepartmentBreakdownDto
                {
                    Department = g.Key,
                    Count = g.Count(),
                    Percentage = totalEmployees == 0 ? 0 : Math.Round(g.Count() * 100m / totalEmployees, 2),
                }));

        var recentEmployees = await repository.ToListAsync(
            query
                .OrderByDescending(e => e.CreatedAt)
                .ThenByDescending(e => e.Id)
                .Take(5)
                .Select(MapToResponseProjection));

        return new DashboardSummaryDto
        {
            TotalEmployees = totalEmployees,
            ActiveEmployees = activeEmployees,
            InactiveEmployees = inactiveEmployees,
            TotalDepartments = departments.Count,
            DepartmentBreakdown = departments,
            RecentEmployees = recentEmployees,
        };
    }

    private static EmployeeQueryParams NormalizeQuery(EmployeeQueryParams queryParams)
    {
        var normalizedPageSize = Math.Clamp(queryParams.PageSize <= 0 ? 10 : queryParams.PageSize, 1, 100);

        return new EmployeeQueryParams
        {
            Search = queryParams.Search,
            Department = NormalizeFilter(queryParams.Department),
            Status = NormalizeFilter(queryParams.Status),
            SortBy = string.IsNullOrWhiteSpace(queryParams.SortBy) ? "name" : queryParams.SortBy.Trim(),
            SortDir = string.Equals(queryParams.SortDir, "desc", StringComparison.OrdinalIgnoreCase) ? "desc" : "asc",
            Page = queryParams.Page <= 0 ? 1 : queryParams.Page,
            PageSize = normalizedPageSize,
        };
    }

    private static string? NormalizeFilter(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) || string.Equals(value, "All", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        return value.Trim();
    }

    private static IQueryable<Employee> ApplySorting(IQueryable<Employee> query, string sortBy, string sortDir)
    {
        var descending = string.Equals(sortDir, "desc", StringComparison.OrdinalIgnoreCase);
        var normalizedSort = sortBy.Trim().ToLowerInvariant();

        return normalizedSort switch
        {
            "salary" => descending
                ? query.OrderByDescending(e => e.Salary).ThenByDescending(e => e.Id)
                : query.OrderBy(e => e.Salary).ThenBy(e => e.Id),
            "joindate" => descending
                ? query.OrderByDescending(e => e.JoinDate).ThenByDescending(e => e.Id)
                : query.OrderBy(e => e.JoinDate).ThenBy(e => e.Id),
            _ => descending
                ? query.OrderByDescending(e => e.LastName).ThenByDescending(e => e.FirstName)
                : query.OrderBy(e => e.LastName).ThenBy(e => e.FirstName),
        };
    }

    private static DateTime NormalizeUtc(DateTime value)
    {
        return value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc),
        };
    }

    private static readonly Expression<Func<Employee, EmployeeResponseDto>> MapToResponseProjection =
        employee => new EmployeeResponseDto
        {
            Id = employee.Id,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Email = employee.Email,
            Phone = employee.Phone,
            Department = employee.Department,
            Designation = employee.Designation,
            Salary = employee.Salary,
            JoinDate = employee.JoinDate,
            Status = employee.Status,
            CreatedAt = employee.CreatedAt,
            UpdatedAt = employee.UpdatedAt,
        };

    private static EmployeeResponseDto MapToResponse(Employee employee) => new()
    {
        Id = employee.Id,
        FirstName = employee.FirstName,
        LastName = employee.LastName,
        Email = employee.Email,
        Phone = employee.Phone,
        Department = employee.Department,
        Designation = employee.Designation,
        Salary = employee.Salary,
        JoinDate = employee.JoinDate,
        Status = employee.Status,
        CreatedAt = employee.CreatedAt,
        UpdatedAt = employee.UpdatedAt,
    };
}

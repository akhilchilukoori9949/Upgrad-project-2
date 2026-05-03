using EMS.API.Data;
using EMS.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EMS.API.Services;

/// <summary>
/// EF Core implementation of the employee repository contract.
/// </summary>
public class EmployeeRepository(AppDbContext context) : IEmployeeRepository
{
    public IQueryable<Employee> Query() => context.Employees.AsQueryable();

    public Task<Employee?> GetByIdAsync(int id) =>
        context.Employees.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);

    public Task<Employee?> GetTrackedByIdAsync(int id) =>
        context.Employees.FirstOrDefaultAsync(e => e.Id == id);

    public Task AddAsync(Employee employee) => context.Employees.AddAsync(employee).AsTask();

    public void Update(Employee employee) => context.Employees.Update(employee);

    public void Remove(Employee employee) => context.Employees.Remove(employee);

    public Task<bool> EmailExistsAsync(string email, int? excludeId = null)
    {
        var normalizedEmail = email.Trim().ToLower();

        return context.Employees.AnyAsync(e =>
            e.Email.ToLower() == normalizedEmail &&
            (!excludeId.HasValue || e.Id != excludeId.Value));
    }

    public Task<int> CountAsync<T>(IQueryable<T> query) => query.CountAsync();

    public Task<List<T>> ToListAsync<T>(IQueryable<T> query) => query.ToListAsync();

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);
}

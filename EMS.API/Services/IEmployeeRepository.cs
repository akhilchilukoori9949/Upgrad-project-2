using EMS.API.Models;

namespace EMS.API.Services;

/// <summary>
/// Data-access abstraction for employee persistence and query execution.
/// </summary>
public interface IEmployeeRepository
{
    IQueryable<Employee> Query();

    Task<Employee?> GetByIdAsync(int id);

    Task<Employee?> GetTrackedByIdAsync(int id);

    Task AddAsync(Employee employee);

    void Update(Employee employee);

    void Remove(Employee employee);

    Task<bool> EmailExistsAsync(string email, int? excludeId = null);

    Task<int> CountAsync<T>(IQueryable<T> query);

    Task<List<T>> ToListAsync<T>(IQueryable<T> query);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

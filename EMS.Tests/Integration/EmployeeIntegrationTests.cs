using EMS.API.Data;
using EMS.API.DTOs;
using EMS.API.Models;
using EMS.API.Services;
using Microsoft.EntityFrameworkCore;

namespace EMS.Tests.Integration;

[TestFixture]
public class EmployeeIntegrationTests
{
    private AppDbContext _db = null!;
    private EmployeeService _service = null!;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _db = new AppDbContext(options);
        _db.Database.EnsureCreated();
        _db.Employees.RemoveRange(_db.Employees);
        _db.AppUsers.RemoveRange(_db.AppUsers);
        _db.SaveChanges();

        _db.Employees.AddRange(
            new Employee
            {
                Id = 1,
                FirstName = "Priya",
                LastName = "Prabhu",
                Email = "priya@test.com",
                Phone = "9876543210",
                Department = "Engineering",
                Designation = "Software Engineer",
                Salary = 850000m,
                JoinDate = new DateTime(2021, 3, 15, 0, 0, 0, DateTimeKind.Utc),
                Status = "Active",
                CreatedAt = DateTime.UtcNow.AddMinutes(-2),
                UpdatedAt = DateTime.UtcNow.AddMinutes(-2),
            },
            new Employee
            {
                Id = 2,
                FirstName = "Arjun",
                LastName = "Sharma",
                Email = "arjun@test.com",
                Phone = "9812345678",
                Department = "Marketing",
                Designation = "Marketing Executive",
                Salary = 620000m,
                JoinDate = new DateTime(2020, 7, 1, 0, 0, 0, DateTimeKind.Utc),
                Status = "Inactive",
                CreatedAt = DateTime.UtcNow.AddMinutes(-1),
                UpdatedAt = DateTime.UtcNow.AddMinutes(-1),
            });
        _db.SaveChanges();

        _service = new EmployeeService(new EmployeeRepository(_db));
    }

    [TearDown]
    public void TearDown()
    {
        _db.Dispose();
    }

    [Test]
    public async Task CreateAsync_AddsEmployeeAndCanBeRetrieved()
    {
        var createResult = await _service.CreateAsync(new EmployeeRequestDto
        {
            FirstName = "Meera",
            LastName = "Krishnan",
            Email = "meera@test.com",
            Phone = "9998887776",
            Department = "HR",
            Designation = "HR Executive",
            Salary = 550000m,
            JoinDate = new DateTime(2024, 1, 1),
            Status = "Active",
        });

        var employee = await _service.GetByIdAsync(createResult.Data!.Id);

        Assert.That(createResult.Success, Is.True);
        Assert.That(employee, Is.Not.Null);
        Assert.That(employee!.Email, Is.EqualTo("meera@test.com"));
    }

    [Test]
    public async Task DeleteAsync_RemovesRecord()
    {
        var deleteResult = await _service.DeleteAsync(1);
        var employee = await _service.GetByIdAsync(1);

        Assert.That(deleteResult.Success, Is.True);
        Assert.That(employee, Is.Null);
    }

    [Test]
    public async Task CreateAsync_DuplicateEmail_ReturnsConflict()
    {
        var result = await _service.CreateAsync(new EmployeeRequestDto
        {
            FirstName = "Another",
            LastName = "Person",
            Email = "priya@test.com",
            Phone = "9998887775",
            Department = "Finance",
            Designation = "Analyst",
            Salary = 610000m,
            JoinDate = new DateTime(2024, 2, 1),
            Status = "Active",
        });

        Assert.That(result.Success, Is.False);
        Assert.That(result.ErrorCode, Is.EqualTo("Conflict"));
    }

    [Test]
    public async Task GetDashboardAsync_ReturnsCorrectCounts()
    {
        var summary = await _service.GetDashboardAsync();

        Assert.That(summary.TotalEmployees, Is.EqualTo(2));
        Assert.That(summary.ActiveEmployees, Is.EqualTo(1));
        Assert.That(summary.InactiveEmployees, Is.EqualTo(1));
        Assert.That(summary.TotalDepartments, Is.EqualTo(2));
    }
}

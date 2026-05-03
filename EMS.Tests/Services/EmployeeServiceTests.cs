using EMS.API.DTOs;
using EMS.API.Models;
using EMS.API.Services;
using Moq;

namespace EMS.Tests.Services;

[TestFixture]
public class EmployeeServiceTests
{
    private Mock<IEmployeeRepository> _repoMock = null!;
    private EmployeeService _service = null!;

    [SetUp]
    public void Setup()
    {
        _repoMock = new Mock<IEmployeeRepository>();
        _service = new EmployeeService(_repoMock.Object);
    }

    [Test]
    public async Task GetByIdAsync_ValidId_ReturnsMappedDto()
    {
        var fakeEmployee = new Employee
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
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(fakeEmployee);

        var result = await _service.GetByIdAsync(1);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.FirstName, Is.EqualTo("Priya"));
        _repoMock.Verify(r => r.GetByIdAsync(1), Times.Once);
    }

    [Test]
    public async Task GetByIdAsync_NonExistentId_ReturnsNull()
    {
        _repoMock.Setup(r => r.GetByIdAsync(9999)).ReturnsAsync((Employee?)null);

        var result = await _service.GetByIdAsync(9999);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task CreateAsync_ValidRequest_CallsAddAsync()
    {
        Employee? capturedEmployee = null;

        _repoMock.Setup(r => r.EmailExistsAsync(It.IsAny<string>(), null)).ReturnsAsync(false);
        _repoMock.Setup(r => r.AddAsync(It.IsAny<Employee>()))
            .Callback<Employee>(employee =>
            {
                employee.Id = 16;
                capturedEmployee = employee;
            })
            .Returns(Task.CompletedTask);
        _repoMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var request = new EmployeeRequestDto
        {
            FirstName = "New",
            LastName = "Employee",
            Email = "new.employee@test.com",
            Phone = "9998887776",
            Department = "Engineering",
            Designation = "Developer",
            Salary = 650000m,
            JoinDate = new DateTime(2024, 1, 1),
            Status = "Active",
        };

        var result = await _service.CreateAsync(request);

        Assert.That(result.Success, Is.True);
        Assert.That(capturedEmployee, Is.Not.Null);
        Assert.That(capturedEmployee!.Email, Is.EqualTo("new.employee@test.com"));
        _repoMock.Verify(r => r.AddAsync(It.IsAny<Employee>()), Times.Once);
        _repoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}

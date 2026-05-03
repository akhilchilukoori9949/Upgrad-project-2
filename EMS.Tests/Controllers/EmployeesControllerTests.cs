using EMS.API.Controllers;
using EMS.API.DTOs;
using EMS.API.Models;
using EMS.API.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace EMS.Tests.Controllers;

[TestFixture]
public class EmployeesControllerTests
{
    [Test]
    public async Task GetById_UnknownId_ReturnsNotFound()
    {
        var repoMock = new Mock<IEmployeeRepository>();
        repoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Employee?)null);

        var service = new EmployeeService(repoMock.Object);
        var controller = new EmployeesController(service);

        var result = await controller.GetById(999, CancellationToken.None);

        Assert.That(result.Result, Is.InstanceOf<NotFoundObjectResult>());
    }

    [Test]
    public async Task GetById_ExistingId_ReturnsOk()
    {
        var repoMock = new Mock<IEmployeeRepository>();
        repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Employee
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
        });

        var service = new EmployeeService(repoMock.Object);
        var controller = new EmployeesController(service);

        var result = await controller.GetById(1, CancellationToken.None);

        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var payload = (result.Result as OkObjectResult)?.Value as EmployeeResponseDto;
        Assert.That(payload, Is.Not.Null);
        Assert.That(payload!.FirstName, Is.EqualTo("Priya"));
    }
}

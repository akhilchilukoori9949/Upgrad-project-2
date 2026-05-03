using EMS.API.Data;
using EMS.API.DTOs;
using EMS.API.Models;
using EMS.API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;

namespace EMS.Tests.Services;

[TestFixture]
public class AuthServiceTests
{
    private AppDbContext _db = null!;
    private Mock<IConfiguration> _configMock = null!;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _db = new AppDbContext(options);
        _db.Database.EnsureCreated();
        _db.AppUsers.RemoveRange(_db.AppUsers);
        _db.SaveChanges();

        _db.AppUsers.Add(new AppUser
        {
            Id = 1,
            Username = "admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123", workFactor: 12),
            Role = "Admin",
            CreatedAt = DateTime.UtcNow,
        });
        _db.SaveChanges();

        _configMock = new Mock<IConfiguration>();
        _configMock.Setup(c => c["Jwt:Key"]).Returns("TestSecretKey_32Chars_ForNUnit!!");
        _configMock.Setup(c => c["Jwt:Issuer"]).Returns("EMS.API");
        _configMock.Setup(c => c["Jwt:Audience"]).Returns("EMS.Client");
        _configMock.Setup(c => c["Jwt:ExpiryHours"]).Returns("8");
    }

    [TearDown]
    public void TearDown()
    {
        _db.Dispose();
    }

    [Test]
    public async Task LoginAsync_ValidCredentials_ReturnsToken()
    {
        var service = new AuthService(_db, _configMock.Object);

        var result = await service.LoginAsync(new LoginRequestDto
        {
            Username = "admin",
            Password = "admin123",
        });

        Assert.That(result.Success, Is.True);
        Assert.That(result.Token, Is.Not.Null.And.Not.Empty);
    }

    [Test]
    public async Task LoginAsync_WrongPassword_ReturnsFailure()
    {
        var service = new AuthService(_db, _configMock.Object);

        var result = await service.LoginAsync(new LoginRequestDto
        {
            Username = "admin",
            Password = "wrongpass",
        });

        Assert.That(result.Success, Is.False);
        Assert.That(result.Token, Is.EqualTo(string.Empty));
    }

    [Test]
    public async Task RegisterAsync_DuplicateUsername_ReturnsFailure()
    {
        var service = new AuthService(_db, _configMock.Object);

        var result = await service.RegisterAsync(new RegisterRequestDto
        {
            Username = "ADMIN",
            Password = "admin123",
            Role = "Admin",
        });

        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("exists"));
    }

    [Test]
    public void GenerateToken_ValidUser_ReturnsNonEmptyString()
    {
        var service = new AuthService(_db, _configMock.Object);

        var token = service.GenerateToken(new AppUser
        {
            Id = 99,
            Username = "viewer",
            Role = "Viewer",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("viewer123", workFactor: 12),
        });

        Assert.That(token, Is.Not.Null.And.Not.Empty);
    }
}

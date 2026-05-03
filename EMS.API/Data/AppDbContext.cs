using EMS.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EMS.API.Data;

/// <summary>
/// EF Core database context for the Employee Management System.
/// </summary>
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    private static readonly DateTime SeedCreatedAt = new(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    private static readonly DateTime SeedUpdatedAt = new(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    private static readonly DateTime SeedUserCreatedAt = new(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public DbSet<Employee> Employees => Set<Employee>();

    public DbSet<AppUser> AppUsers => Set<AppUser>();

    public override int SaveChanges()
    {
        ApplyAuditFields();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditFields();
        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Salary).HasColumnType("decimal(18,2)");
            entity.Property(e => e.FirstName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.LastName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Phone).HasMaxLength(15).IsRequired();
            entity.Property(e => e.Department).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Designation).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Status).HasMaxLength(10).IsRequired();
        });

        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.HasIndex(u => u.Username).IsUnique();
            entity.Property(u => u.Username).HasMaxLength(100).IsRequired();
            entity.Property(u => u.Role).HasMaxLength(20).IsRequired();
            entity.Property(u => u.PasswordHash).IsRequired();
        });

        modelBuilder.Entity<Employee>().HasData(GetSeedEmployees());
        modelBuilder.Entity<AppUser>().HasData(GetSeedUsers());
    }

    private void ApplyAuditFields()
    {
        var utcNow = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<Employee>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = utcNow;
                entry.Entity.UpdatedAt = utcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Property(e => e.CreatedAt).IsModified = false;
                entry.Entity.UpdatedAt = utcNow;
            }
        }

        foreach (var entry in ChangeTracker.Entries<AppUser>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = utcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Property(e => e.CreatedAt).IsModified = false;
            }
        }
    }

    private static IEnumerable<Employee> GetSeedEmployees()
    {
        return
        [
            new Employee { Id = 1, FirstName = "Priya", LastName = "Prabhu", Email = "priya.prabhu@xyz.com", Phone = "9876543210", Department = "Engineering", Designation = "Software Engineer", Salary = 850000m, JoinDate = new DateTime(2021, 3, 15, 0, 0, 0, DateTimeKind.Utc), Status = "Active", CreatedAt = SeedCreatedAt.AddMinutes(1), UpdatedAt = SeedUpdatedAt.AddMinutes(1) },
            new Employee { Id = 2, FirstName = "Arjun", LastName = "Sharma", Email = "arjun.sharma@xyz.com", Phone = "9812345678", Department = "Marketing", Designation = "Marketing Executive", Salary = 620000m, JoinDate = new DateTime(2020, 7, 1, 0, 0, 0, DateTimeKind.Utc), Status = "Active", CreatedAt = SeedCreatedAt.AddMinutes(2), UpdatedAt = SeedUpdatedAt.AddMinutes(2) },
            new Employee { Id = 3, FirstName = "Neha", LastName = "Kapoor", Email = "neha.kapoor@xyz.com", Phone = "9823456789", Department = "HR", Designation = "HR Executive", Salary = 550000m, JoinDate = new DateTime(2019, 11, 20, 0, 0, 0, DateTimeKind.Utc), Status = "Active", CreatedAt = SeedCreatedAt.AddMinutes(3), UpdatedAt = SeedUpdatedAt.AddMinutes(3) },
            new Employee { Id = 4, FirstName = "Rahul", LastName = "Verma", Email = "rahul.verma@xyz.com", Phone = "9834567890", Department = "Finance", Designation = "Financial Analyst", Salary = 720000m, JoinDate = new DateTime(2022, 1, 10, 0, 0, 0, DateTimeKind.Utc), Status = "Active", CreatedAt = SeedCreatedAt.AddMinutes(4), UpdatedAt = SeedUpdatedAt.AddMinutes(4) },
            new Employee { Id = 5, FirstName = "Sneha", LastName = "Prasad", Email = "sneha.prasad@xyz.com", Phone = "9845678901", Department = "Operations", Designation = "Operations Manager", Salary = 950000m, JoinDate = new DateTime(2018, 6, 5, 0, 0, 0, DateTimeKind.Utc), Status = "Active", CreatedAt = SeedCreatedAt.AddMinutes(5), UpdatedAt = SeedUpdatedAt.AddMinutes(5) },
            new Employee { Id = 6, FirstName = "Vikram", LastName = "Raj", Email = "vikram.raj@xyz.com", Phone = "9856789012", Department = "Engineering", Designation = "Senior Developer", Salary = 1100000m, JoinDate = new DateTime(2017, 9, 12, 0, 0, 0, DateTimeKind.Utc), Status = "Active", CreatedAt = SeedCreatedAt.AddMinutes(6), UpdatedAt = SeedUpdatedAt.AddMinutes(6) },
            new Employee { Id = 7, FirstName = "Ananya", LastName = "Singh", Email = "ananya.singh@xyz.com", Phone = "9867890123", Department = "Marketing", Designation = "Content Strategist", Salary = 580000m, JoinDate = new DateTime(2023, 2, 28, 0, 0, 0, DateTimeKind.Utc), Status = "Inactive", CreatedAt = SeedCreatedAt.AddMinutes(7), UpdatedAt = SeedUpdatedAt.AddMinutes(7) },
            new Employee { Id = 8, FirstName = "Karthik", LastName = "Rajan", Email = "karthik.rajan@xyz.com", Phone = "9878901234", Department = "Finance", Designation = "Accounts Manager", Salary = 800000m, JoinDate = new DateTime(2020, 4, 17, 0, 0, 0, DateTimeKind.Utc), Status = "Active", CreatedAt = SeedCreatedAt.AddMinutes(8), UpdatedAt = SeedUpdatedAt.AddMinutes(8) },
            new Employee { Id = 9, FirstName = "Divya", LastName = "Kumar", Email = "divya.kumar@xyz.com", Phone = "9889012345", Department = "HR", Designation = "Talent Acquisition", Salary = 690000m, JoinDate = new DateTime(2021, 8, 22, 0, 0, 0, DateTimeKind.Utc), Status = "Active", CreatedAt = SeedCreatedAt.AddMinutes(9), UpdatedAt = SeedUpdatedAt.AddMinutes(9) },
            new Employee { Id = 10, FirstName = "Rohan", LastName = "Mehta", Email = "rohan.mehta@xyz.com", Phone = "9890123456", Department = "Engineering", Designation = "DevOps Engineer", Salary = 920000m, JoinDate = new DateTime(2022, 5, 3, 0, 0, 0, DateTimeKind.Utc), Status = "Active", CreatedAt = SeedCreatedAt.AddMinutes(10), UpdatedAt = SeedUpdatedAt.AddMinutes(10) },
            new Employee { Id = 11, FirstName = "Lakshmi", LastName = "Chandran", Email = "lakshmi.chandran@xyz.com", Phone = "9801234567", Department = "Marketing", Designation = "Brand Manager", Salary = 750000m, JoinDate = new DateTime(2021, 12, 11, 0, 0, 0, DateTimeKind.Utc), Status = "Active", CreatedAt = SeedCreatedAt.AddMinutes(11), UpdatedAt = SeedUpdatedAt.AddMinutes(11) },
            new Employee { Id = 12, FirstName = "Suresh", LastName = "Babu", Email = "suresh.babu@xyz.com", Phone = "9811223344", Department = "Finance", Designation = "Tax Consultant", Salary = 680000m, JoinDate = new DateTime(2019, 3, 25, 0, 0, 0, DateTimeKind.Utc), Status = "Inactive", CreatedAt = SeedCreatedAt.AddMinutes(12), UpdatedAt = SeedUpdatedAt.AddMinutes(12) },
            new Employee { Id = 13, FirstName = "Meera", LastName = "Krishnan", Email = "meera.krishnan@xyz.com", Phone = "9822334455", Department = "Engineering", Designation = "QA Engineer", Salary = 710000m, JoinDate = new DateTime(2020, 10, 8, 0, 0, 0, DateTimeKind.Utc), Status = "Active", CreatedAt = SeedCreatedAt.AddMinutes(13), UpdatedAt = SeedUpdatedAt.AddMinutes(13) },
            new Employee { Id = 14, FirstName = "Amit", LastName = "Joshi", Email = "amit.joshi@xyz.com", Phone = "9833445566", Department = "Operations", Designation = "Supply Chain Analyst", Salary = 630000m, JoinDate = new DateTime(2023, 7, 14, 0, 0, 0, DateTimeKind.Utc), Status = "Active", CreatedAt = SeedCreatedAt.AddMinutes(14), UpdatedAt = SeedUpdatedAt.AddMinutes(14) },
            new Employee { Id = 15, FirstName = "Pooja", LastName = "Ghosh", Email = "pooja.ghosh@xyz.com", Phone = "9844556677", Department = "Operations", Designation = "Process Engineer", Salary = 870000m, JoinDate = new DateTime(2024, 1, 20, 0, 0, 0, DateTimeKind.Utc), Status = "Inactive", CreatedAt = SeedCreatedAt.AddMinutes(15), UpdatedAt = SeedUpdatedAt.AddMinutes(15) },
        ];
    }

    private static IEnumerable<AppUser> GetSeedUsers()
    {
        return
        [
            new AppUser
            {
                Id = 1,
                Username = "admin",
                PasswordHash = "$2a$12$3K17kjT.g9XdQx6Kl3ooCulOPQN1U5PckcDF.E/xm0cqSNXFjr/2G",
                Role = "Admin",
                CreatedAt = SeedUserCreatedAt.AddMinutes(1),
            },
            new AppUser
            {
                Id = 2,
                Username = "viewer",
                PasswordHash = "$2a$12$XLRrpiYqqC.5/.QecCzTFeNGa.FHhv/QTPntyVxYvTm2.kBCcv2lm",
                Role = "Viewer",
                CreatedAt = SeedUserCreatedAt.AddMinutes(2),
            },
        ];
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EMS.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Department = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Salary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    JoinDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "AppUsers",
                columns: new[] { "Id", "CreatedAt", "PasswordHash", "Role", "Username" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 1, 0, 0, DateTimeKind.Utc), "$2a$12$3K17kjT.g9XdQx6Kl3ooCulOPQN1U5PckcDF.E/xm0cqSNXFjr/2G", "Admin", "admin" },
                    { 2, new DateTime(2024, 1, 1, 0, 2, 0, 0, DateTimeKind.Utc), "$2a$12$XLRrpiYqqC.5/.QecCzTFeNGa.FHhv/QTPntyVxYvTm2.kBCcv2lm", "Viewer", "viewer" }
                });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "CreatedAt", "Department", "Designation", "Email", "FirstName", "JoinDate", "LastName", "Phone", "Salary", "Status", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 1, 0, 0, DateTimeKind.Utc), "Engineering", "Software Engineer", "priya.prabhu@xyz.com", "Priya", new DateTime(2021, 3, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Prabhu", "9876543210", 850000m, "Active", new DateTime(2024, 1, 1, 0, 1, 0, 0, DateTimeKind.Utc) },
                    { 2, new DateTime(2024, 1, 1, 0, 2, 0, 0, DateTimeKind.Utc), "Marketing", "Marketing Executive", "arjun.sharma@xyz.com", "Arjun", new DateTime(2020, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Sharma", "9812345678", 620000m, "Active", new DateTime(2024, 1, 1, 0, 2, 0, 0, DateTimeKind.Utc) },
                    { 3, new DateTime(2024, 1, 1, 0, 3, 0, 0, DateTimeKind.Utc), "HR", "HR Executive", "neha.kapoor@xyz.com", "Neha", new DateTime(2019, 11, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Kapoor", "9823456789", 550000m, "Active", new DateTime(2024, 1, 1, 0, 3, 0, 0, DateTimeKind.Utc) },
                    { 4, new DateTime(2024, 1, 1, 0, 4, 0, 0, DateTimeKind.Utc), "Finance", "Financial Analyst", "rahul.verma@xyz.com", "Rahul", new DateTime(2022, 1, 10, 0, 0, 0, 0, DateTimeKind.Utc), "Verma", "9834567890", 720000m, "Active", new DateTime(2024, 1, 1, 0, 4, 0, 0, DateTimeKind.Utc) },
                    { 5, new DateTime(2024, 1, 1, 0, 5, 0, 0, DateTimeKind.Utc), "Operations", "Operations Manager", "sneha.prasad@xyz.com", "Sneha", new DateTime(2018, 6, 5, 0, 0, 0, 0, DateTimeKind.Utc), "Prasad", "9845678901", 950000m, "Active", new DateTime(2024, 1, 1, 0, 5, 0, 0, DateTimeKind.Utc) },
                    { 6, new DateTime(2024, 1, 1, 0, 6, 0, 0, DateTimeKind.Utc), "Engineering", "Senior Developer", "vikram.raj@xyz.com", "Vikram", new DateTime(2017, 9, 12, 0, 0, 0, 0, DateTimeKind.Utc), "Raj", "9856789012", 1100000m, "Active", new DateTime(2024, 1, 1, 0, 6, 0, 0, DateTimeKind.Utc) },
                    { 7, new DateTime(2024, 1, 1, 0, 7, 0, 0, DateTimeKind.Utc), "Marketing", "Content Strategist", "ananya.singh@xyz.com", "Ananya", new DateTime(2023, 2, 28, 0, 0, 0, 0, DateTimeKind.Utc), "Singh", "9867890123", 580000m, "Inactive", new DateTime(2024, 1, 1, 0, 7, 0, 0, DateTimeKind.Utc) },
                    { 8, new DateTime(2024, 1, 1, 0, 8, 0, 0, DateTimeKind.Utc), "Finance", "Accounts Manager", "karthik.rajan@xyz.com", "Karthik", new DateTime(2020, 4, 17, 0, 0, 0, 0, DateTimeKind.Utc), "Rajan", "9878901234", 800000m, "Active", new DateTime(2024, 1, 1, 0, 8, 0, 0, DateTimeKind.Utc) },
                    { 9, new DateTime(2024, 1, 1, 0, 9, 0, 0, DateTimeKind.Utc), "HR", "Talent Acquisition", "divya.kumar@xyz.com", "Divya", new DateTime(2021, 8, 22, 0, 0, 0, 0, DateTimeKind.Utc), "Kumar", "9889012345", 690000m, "Active", new DateTime(2024, 1, 1, 0, 9, 0, 0, DateTimeKind.Utc) },
                    { 10, new DateTime(2024, 1, 1, 0, 10, 0, 0, DateTimeKind.Utc), "Engineering", "DevOps Engineer", "rohan.mehta@xyz.com", "Rohan", new DateTime(2022, 5, 3, 0, 0, 0, 0, DateTimeKind.Utc), "Mehta", "9890123456", 920000m, "Active", new DateTime(2024, 1, 1, 0, 10, 0, 0, DateTimeKind.Utc) },
                    { 11, new DateTime(2024, 1, 1, 0, 11, 0, 0, DateTimeKind.Utc), "Marketing", "Brand Manager", "lakshmi.chandran@xyz.com", "Lakshmi", new DateTime(2021, 12, 11, 0, 0, 0, 0, DateTimeKind.Utc), "Chandran", "9801234567", 750000m, "Active", new DateTime(2024, 1, 1, 0, 11, 0, 0, DateTimeKind.Utc) },
                    { 12, new DateTime(2024, 1, 1, 0, 12, 0, 0, DateTimeKind.Utc), "Finance", "Tax Consultant", "suresh.babu@xyz.com", "Suresh", new DateTime(2019, 3, 25, 0, 0, 0, 0, DateTimeKind.Utc), "Babu", "9811223344", 680000m, "Inactive", new DateTime(2024, 1, 1, 0, 12, 0, 0, DateTimeKind.Utc) },
                    { 13, new DateTime(2024, 1, 1, 0, 13, 0, 0, DateTimeKind.Utc), "Engineering", "QA Engineer", "meera.krishnan@xyz.com", "Meera", new DateTime(2020, 10, 8, 0, 0, 0, 0, DateTimeKind.Utc), "Krishnan", "9822334455", 710000m, "Active", new DateTime(2024, 1, 1, 0, 13, 0, 0, DateTimeKind.Utc) },
                    { 14, new DateTime(2024, 1, 1, 0, 14, 0, 0, DateTimeKind.Utc), "Operations", "Supply Chain Analyst", "amit.joshi@xyz.com", "Amit", new DateTime(2023, 7, 14, 0, 0, 0, 0, DateTimeKind.Utc), "Joshi", "9833445566", 630000m, "Active", new DateTime(2024, 1, 1, 0, 14, 0, 0, DateTimeKind.Utc) },
                    { 15, new DateTime(2024, 1, 1, 0, 15, 0, 0, DateTimeKind.Utc), "Operations", "Process Engineer", "pooja.ghosh@xyz.com", "Pooja", new DateTime(2024, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Ghosh", "9844556677", 870000m, "Inactive", new DateTime(2024, 1, 1, 0, 15, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppUsers_Username",
                table: "AppUsers",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Email",
                table: "Employees",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppUsers");

            migrationBuilder.DropTable(
                name: "Employees");
        }
    }
}

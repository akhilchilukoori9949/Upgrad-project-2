# B6 - Chilukoori Akhil - Employee Management System Mini Project 2

## Project Structure
- `EMS.API/` - .NET 8 Web API with Entity Framework Core, SQL Server, JWT auth, Swagger, and migrations.
- `EMS.Tests/` - NUnit + Moq + EF Core InMemory tests.
- `frontend/` - Updated Mini Project 1 frontend wired to the API.
- `EMS-MiniProject2.sln` - Solution file for Visual Studio.

## Default Users
- `admin / admin123` - Admin role
- `viewer / viewer123` - Viewer role

## Prerequisites
- .NET 8 SDK
- SQL Server 2022 (local or Express)
- Node.js 18+ for frontend Jest tests
- Live Server extension in VS Code (optional but recommended)

## Connection String
The API uses this default connection string in `EMS.API/appsettings.json`:

```json
"DefaultConnection": "Server=localhost;Database=EMSDashboard;Trusted_Connection=True;TrustServerCertificate=True;"
```

If you use SQL Server Express, change it to:

```json
"DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=EMSDashboard;Trusted_Connection=True;TrustServerCertificate=True;"
```

## How To Run
1. Open the solution in Visual Studio or terminal at the project root.
2. Restore the local EF tool:
   ```powershell
   dotnet tool restore
   ```
3. Apply the migration before first run:
   ```powershell
   dotnet ef database update --project EMS.API --startup-project EMS.API
   ```
4. Run the API:
   ```powershell
   dotnet run --project EMS.API
   ```
5. Open Swagger:
   - `http://localhost:5000/swagger`
6. Open the frontend:
   - Open `frontend/index.html` with Live Server, or open it directly in the browser.

## Frontend Tests (Mini Project 1 Jest Tests)
Run from the `frontend` folder:

```powershell
npm install
npm test -- --runInBand
```

## Backend Tests
Run from the project root:

```powershell
$env:DOTNET_ROLL_FORWARD='Major'
dotnet test EMS.Tests\EMS.Tests.csproj -c Debug --no-build --no-restore
```

If your machine already has the .NET 8 runtime installed, the `DOTNET_ROLL_FORWARD` line is not required.

## Verified In This Workspace
- `dotnet build EMS-MiniProject2.sln -c Debug`
- `dotnet test EMS.Tests\EMS.Tests.csproj -c Debug --no-build --no-restore` with `DOTNET_ROLL_FORWARD=Major`
- `npm test -- --runInBand`
- `dotnet publish EMS.API\EMS.API.csproj -c Release -o <published-folder>`

## Notes
- JWT tokens are stored only in memory on the frontend.
- Role-based UI hides Add, Edit, and Delete for Viewer accounts.
- Filtering, sorting, search, and pagination are handled server-side.
- Migration files are already included under `EMS.API/Migrations`.

## 0. Overview

1. Install .NET SDK
2. Spin up **SQL Server** in Docker
3. Scaffold a minimal ASP.NET Core app
4. Connect it to SQL Server
5. Create a simple **HR system** entity (`Employee`)
6. Apply migrations, seed data
7. Run the app and test CRUD

---

## 🐧 Step 1. Install .NET 8 SDK

On Fedora:

```bash
sudo dnf install dotnet-sdk-8.0
dotnet --version
```
(Should print something like `8.0.x`)

For database migrations, also install EF Core tools:

```bash
dotnet tool install --global dotnet-ef
dotnet ef --version
```

(Should print something like)
```
Entity Framework Core .NET Command-line Tools
9.0.9
```


---

## 🐳 Step 2. Start SQL Server in Docker

```bash
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=Your_strong_123" \
   -p 1433:1433 --name sqlserver -d mcr.microsoft.com/mssql/server:2022-latest
```

Check it’s running:

```bash
docker ps
```

---

## 📂 Step 3. Create Project

```bash
dotnet new webapp -o HrLabApp
cd HrLabApp
```

Run it once:

```bash
dotnet run
```

Visit [http://localhost:5000](http://localhost:5000) to confirm.

---

## 📦 Step 4. Add EF Core & SQL Server

```bash
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
```

---

## 📝 Step 5. Add Model & DbContext

### `Models/Employee.cs`

```csharp
namespace HrLabApp.Models;

public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public decimal Salary { get; set; }
}
```

### `Data/AppDbContext.cs`

```csharp
using Microsoft.EntityFrameworkCore;
using HrLabApp.Models;

namespace HrLabApp.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Employee> Employees { get; set; }
}
```

---

## ⚙️ Step 6. Configure SQL Connection

### `appsettings.json` → add:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost,1433;Database=HrLabDb;User Id=sa;Password=Your_strong_123;TrustServerCertificate=True;"
}
```

### `Program.cs` → update:

```csharp
using HrLabApp.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapRazorPages();
app.Run();
```

---

## 🧱 Step 7. Create Pages for Employees

```bash
dotnet new page -n Employees/Index -o Pages/Employees
dotnet new page -n Employees/Create -o Pages/Employees
dotnet new page -n Employees/Edit -o Pages/Employees
dotnet new page -n Employees/Delete -o Pages/Employees
dotnet new page -n Employees/Details -o Pages/Employees
```

👉 Instead of hand-coding Razor Pages, you can prompt AI:

> “Generate Razor Pages CRUD for `Employee` model using EF Core in ASP.NET Core 8.”

Paste generated code into `Pages/Employees`.

*(If you want, I can send you one ready CRUD Razor Page file to copy-paste.)*

---

## 🗄️ Step 8. Apply Migration & Seed DB

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

To seed dummy employees, modify `Program.cs` **before `app.Run();`**:

```csharp
using HrLabApp.Models;
using HrLabApp.Data;

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (!db.Employees.Any())
    {
        db.Employees.AddRange(
            new Employee { Name = "Alice", Department = "HR", Salary = 50000 },
            new Employee { Name = "Bob", Department = "IT", Salary = 60000 }
        );
        db.SaveChanges();
    }
}
```

---

## 🚀 Step 9. Run App

```bash
dotnet run
```

Visit:

* [http://localhost:5000/Employees](http://localhost:5000/Employees) → list of employees
* Add/Edit/Delete employees

Verify with:

```bash
docker exec -it sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P Your_strong_123 -Q "SELECT * FROM HrLabDb.dbo.Employees;"
```

---

## 🎯 Outcome

You now have:

* ASP.NET Core app running on Fedora
* Connected to SQL Server (Docker)
* CRUD UI for `Employee` table seeded with data

---

## CRUD Razor Pages for `Employee`

### Project Structure
```
HrLabApp/
 ├─ Data/
 │   └─ AppDbContext.cs
 ├─ Models/
 │   └─ Employee.cs
 ├─ Pages/
 │   ├─ Employees/
 │   │   ├─ Index.cshtml
 │   │   ├─ Index.cshtml.cs
 │   │   ├─ Create.cshtml
 │   │   ├─ Create.cshtml.cs
 │   │   ├─ Edit.cshtml
 │   │   ├─ Edit.cshtml.cs
 │   │   ├─ Delete.cshtml
 │   │   ├─ Delete.cshtml.cs
 │   │   ├─ Details.cshtml
 │   │   └─ Details.cshtml.cs
 ├─ Program.cs
 └─ appsettings.json
```

---

## 1. `Index.cshtml`

```html
@page
@model HrLabApp.Pages.Employees.IndexModel

<h1>Employees</h1>

<p>
    <a asp-page="Create">Create New</a>
</p>

<table class="table">
    <thead>
        <tr>
            <th>Name</th>
            <th>Department</th>
            <th>Salary</th>
            <th></th>
        </tr>
    </thead>
    <tbody>
@foreach (var item in Model.EmployeeList) {
        <tr>
            <td>@item.Name</td>
            <td>@item.Department</td>
            <td>@item.Salary</td>
            <td>
                <a asp-page="Edit" asp-route-id="@item.Id">Edit</a> |
                <a asp-page="Details" asp-route-id="@item.Id">Details</a> |
                <a asp-page="Delete" asp-route-id="@item.Id">Delete</a>
            </td>
        </tr>
}
    </tbody>
</table>
```

### `Index.cshtml.cs`

```csharp
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HrLabApp.Data;
using HrLabApp.Models;

namespace HrLabApp.Pages.Employees;

public class IndexModel : PageModel
{
    private readonly AppDbContext _context;

    public IndexModel(AppDbContext context)
    {
        _context = context;
    }

    public IList<Employee> EmployeeList { get; set; } = new List<Employee>();

    public async Task OnGetAsync()
    {
        EmployeeList = await _context.Employees.ToListAsync();
    }
}
```

---

## 2. `Create.cshtml`

```html
@page
@model HrLabApp.Pages.Employees.CreateModel

<h1>Create Employee</h1>

<form method="post">
    <div>
        <label>Name</label>
        <input asp-for="Employee.Name" />
    </div>
    <div>
        <label>Department</label>
        <input asp-for="Employee.Department" />
    </div>
    <div>
        <label>Salary</label>
        <input asp-for="Employee.Salary" />
    </div>
    <button type="submit">Create</button>
</form>

<a asp-page="Index">Back to List</a>
```

### `Create.cshtml.cs`

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HrLabApp.Data;
using HrLabApp.Models;

namespace HrLabApp.Pages.Employees;

public class CreateModel : PageModel
{
    private readonly AppDbContext _context;

    public CreateModel(AppDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Employee Employee { get; set; } = new();

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        _context.Employees.Add(Employee);
        await _context.SaveChangesAsync();

        return RedirectToPage("Index");
    }
}
```

---

## 3. `Edit.cshtml`

```html
@page "{id:int}"
@model HrLabApp.Pages.Employees.EditModel

<h1>Edit Employee</h1>

<form method="post">
    <input type="hidden" asp-for="Employee.Id" />
    <div>
        <label>Name</label>
        <input asp-for="Employee.Name" />
    </div>
    <div>
        <label>Department</label>
        <input asp-for="Employee.Department" />
    </div>
    <div>
        <label>Salary</label>
        <input asp-for="Employee.Salary" />
    </div>
    <button type="submit">Save</button>
</form>

<a asp-page="Index">Back to List</a>
```

### `Edit.cshtml.cs`

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HrLabApp.Data;
using HrLabApp.Models;

namespace HrLabApp.Pages.Employees;

public class EditModel : PageModel
{
    private readonly AppDbContext _context;

    public EditModel(AppDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Employee Employee { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var emp = await _context.Employees.FindAsync(id);
        if (emp == null) return NotFound();
        Employee = emp;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        _context.Attach(Employee).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return RedirectToPage("Index");
    }
}
```

---

## 4. `Delete.cshtml`

```html
@page "{id:int}"
@model HrLabApp.Pages.Employees.DeleteModel

<h1>Delete Employee</h1>

<div>
    <h3>Are you sure you want to delete this employee?</h3>
    <div>
        <strong>@Model.Employee.Name</strong> -
        @Model.Employee.Department -
        @Model.Employee.Salary
    </div>
</div>

<form method="post">
    <input type="hidden" asp-for="Employee.Id" />
    <button type="submit">Delete</button>
    <a asp-page="Index">Cancel</a>
</form>
```

### `Delete.cshtml.cs`

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HrLabApp.Data;
using HrLabApp.Models;

namespace HrLabApp.Pages.Employees;

public class DeleteModel : PageModel
{
    private readonly AppDbContext _context;

    public DeleteModel(AppDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Employee Employee { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var emp = await _context.Employees.FindAsync(id);
        if (emp == null) return NotFound();
        Employee = emp;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var emp = await _context.Employees.FindAsync(Employee.Id);
        if (emp != null)
        {
            _context.Employees.Remove(emp);
            await _context.SaveChangesAsync();
        }
        return RedirectToPage("Index");
    }
}
```

---

## 5. `Details.cshtml`

```html
@page "{id:int}"
@model HrLabApp.Pages.Employees.DetailsModel

<h1>Employee Details</h1>

<div>
    <p><strong>Name:</strong> @Model.Employee.Name</p>
    <p><strong>Department:</strong> @Model.Employee.Department</p>
    <p><strong>Salary:</strong> @Model.Employee.Salary</p>
</div>

<a asp-page="Edit" asp-route-id="@Model.Employee.Id">Edit</a> |
<a asp-page="Index">Back to List</a>
```

### `Details.cshtml.cs`

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HrLabApp.Data;
using HrLabApp.Models;

namespace HrLabApp.Pages.Employees;

public class DetailsModel : PageModel
{
    private readonly AppDbContext _context;

    public DetailsModel(AppDbContext context)
    {
        _context = context;
    }

    public Employee Employee { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var emp = await _context.Employees.FindAsync(id);
        if (emp == null) return NotFound();
        Employee = emp;
        return Page();
    }
}
```

---

## 🚀 Final Steps

1. Run migrations (again if you haven’t yet):

   ```bash
   dotnet ef migrations add Init
   dotnet ef database update
   ```
2. Seed sample employees (already in the snippet I gave you for `Program.cs`).
3. Start the app:

   ```bash
   dotnet run
   ```
4. Visit: [http://localhost:5000/Employees](http://localhost:5000/Employees)

You should see a **fully working CRUD HR employee system** backed by SQL Server in Docker. 🎉
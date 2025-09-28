# 📘 ASP.NET Core + SQL Server Lab

**Course:** Databases
**Lab Goal:** Build a minimal ASP.NET Core web app that connects to Microsoft SQL Server and implements basic CRUD for one table, using AI assistance.

---

## 🎯 Objectives

By the end of this lab, students will:

* Understand the **basic structure** of an ASP.NET Core web application.
* Connect a web app to **SQL Server** using **Entity Framework Core**.
* Scaffold a CRUD interface (list, add, edit, delete records).
* Use an **LLM (ChatGPT/Copilot/etc.)** to generate code scaffolding.
* See how database concepts translate into a working web application.

---

## 🛠️ System Preparation

### Software Dependencies

1. **.NET 8 SDK (LTS)**

   * Download: [dotnet.microsoft.com/download](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
   * Includes `dotnet` CLI, compiler, runtime, and Kestrel web server.
   * For database work, also install the EF Core CLI tools:

     ```bash
     dotnet tool install --global dotnet-ef
     ```

2. **Visual Studio Code** (cross-platform IDE)

   * Extensions:

     * *C# Dev Kit* (Microsoft)
     * *SQL Server (mssql)* (Microsoft, optional, for querying DB directly)

3. **Microsoft SQL Server**

   * Lab machines should have either:

     * **Local SQL Server instance** (Express or Developer edition), or
     * **Shared SQL Server on LAN** with login credentials provided.
   * Also install **SQL Server Management Studio (SSMS)** or Azure Data Studio for inspection.

4. **Entity Framework Core packages** (installed per project):

   ```bash
   dotnet add package Microsoft.EntityFrameworkCore
   dotnet add package Microsoft.EntityFrameworkCore.SqlServer
   dotnet add package Microsoft.EntityFrameworkCore.Tools
   ```

---

## 🧑‍💻 Lab Outline (Single Session)

### Step 1. Scaffold the Web Application

```bash
dotnet new webapp -o StudentLabApp
cd StudentLabApp
dotnet run
```

* Opens a minimal **Razor Pages** app.
* Visit `https://localhost:5001` to confirm.

---

### Step 2. Add a Database Model

* Example entity: `Student (Id, Name, GPA)`.
* Create `Models/Student.cs`:

  ```csharp
  namespace StudentLabApp.Models;

  public class Student
  {
      public int Id { get; set; }
      public string Name { get; set; } = string.Empty;
      public decimal GPA { get; set; }
  }
  ```

---

### Step 3. Setup EF Core DbContext

* Create `Data/AppDbContext.cs`:

  ```csharp
  using Microsoft.EntityFrameworkCore;
  using StudentLabApp.Models;

  namespace StudentLabApp.Data;

  public class AppDbContext : DbContext
  {
      public AppDbContext(DbContextOptions<AppDbContext> options)
          : base(options) { }

      public DbSet<Student> Students { get; set; }
  }
  ```

---

### Step 4. Configure SQL Server Connection

* In `appsettings.json`, add connection string:

  ```json
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=StudentLabDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
  ```

  *(Adjust for shared DB / lab credentials.)*

* In `Program.cs`, register EF Core:

  ```csharp
  using StudentLabApp.Data;
  using Microsoft.EntityFrameworkCore;

  var builder = WebApplication.CreateBuilder(args);

  builder.Services.AddRazorPages();
  builder.Services.AddDbContext<AppDbContext>(options =>
      options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

  var app = builder.Build();
  ```

---

### Step 5. Apply Migration

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

* Creates `StudentLabDb` with `Students` table in SQL Server.

---

### Step 6. Scaffold CRUD Razor Pages

Use AI prompt:

> “Generate Razor Pages CRUD for `Student` model using EF Core in ASP.NET Core 8.”

Or use CLI scaffolder (optional, requires `dotnet-aspnet-codegenerator`).

Place generated pages under `/Pages/Students`.

---

### Step 7. Run and Test

```bash
dotnet run
```

* Navigate to `/Students`.
* Add/edit/delete student records.
* Verify data persistence in SQL Server (via SSMS query).

---

## 📑 Student Activities

1. **Setup environment** (run sample app).
2. **Define model** (`Student`).
3. **Configure DbContext** with SQL Server.
4. **Apply migration** (create table in DB).
5. **Use AI to scaffold CRUD pages**.
6. **Run & verify CRUD works**.
7. **(Optional extension)** Add another entity (`Course`) with relationship.

---

## ✅ Expected Outcomes

* Students can run an ASP.NET Core app locally.
* A `Student` table is created in SQL Server.
* CRUD operations work via a simple web UI.
* Students understand how:

  * Models map to tables
  * CRUD maps to SQL commands
  * Web apps integrate with databases

---

## 📦 Deliverables

At the end of the lab, students should submit:

* The working ASP.NET project folder (zipped).
* Screenshot of `/Students` page with at least 2 inserted records.
* Screenshot of SQL Server table showing the same records.

---

⚡ **Instructor note:** The complexity is intentionally capped. The “AI-powered” angle means students don’t have to handwrite every Razor page, but they must **understand the mapping between DB concepts and the web app**.
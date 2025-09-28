## 1. Project Structure at a High Level

ASP.NET Core MVC apps follow a **Model–View–Controller** (MVC) + Razor Pages pattern.
Your project contains:

- **`Program.cs`** – the app’s entrypoint. Configures middleware (routing, static files, MVC, EF Core, etc).
- **`appsettings.json`** – config file (connection strings, app config, logging).
- **`Models/`** – classes that represent your database entities (like `Employee.cs`).
- **`Data/`** – the EF Core `DbContext` class (`AppDbContext.cs`), which is the bridge between your models and the SQL database.
- **`Migrations/`** – EF Core-generated C# migrations that create/update the database schema.
- **`Pages/`** – Razor Pages (each `.cshtml` + `.cshtml.cs` is basically View + PageModel).
- **`wwwroot/`** – static web assets (CSS, JS, images, libs like Bootstrap/jQuery).
- **`Properties/launchSettings.json`** – how the app runs locally (ports, profiles).
- **`obj/` + `bin/`** – build outputs (like `node_modules/dist` in JS world).

---

## 2. How Requests Flow

When a user requests `/Employees/Create`:

1. **Routing (Program.cs)**
   ASP.NET Core’s routing middleware maps `/Employees/Create` → `Pages/Employees/Create.cshtml` + `Create.cshtml.cs`.

2. **Page Model (`Create.cshtml.cs`)**
   This C# class acts like a controller. It has:

   - `OnGet()` → called for HTTP GET (renders the form).
   - `OnPostAsync()` → called for HTTP POST (when user submits form).

   Inside `OnPostAsync()`, it uses the injected `AppDbContext` to:

   ```csharp
   _context.Employees.Add(Employee);
   await _context.SaveChangesAsync();
   return RedirectToPage("./Index");
   ```

   That inserts a new row in the SQL Server DB.

3. **View (`Create.cshtml`)**
   A Razor template that:

   - Defines an HTML form.
   - Uses Razor tag helpers like `<input asp-for="Employee.Name">` (auto binds to your C# model).
   - On submit, posts back to the `OnPostAsync()` handler.

So the “Create page” = **Razor View + PageModel (controller logic) + Entity Framework DB interaction**.

---

## 3. Database Integration

- **`Employee.cs`** (in `Models/`) defines the entity fields (Id, Name, Position, etc).
- **`AppDbContext.cs`** wires that up to EF Core:

  ```csharp
  public DbSet<Employee> Employees { get; set; }
  ```

- Migrations (`Migrations/2025xxxx_InitialCreate.cs`) are generated automatically from these models.
- SQL Server container (`docker run ... mcr.microsoft.com/mssql/server`) hosts the actual DB.

---

## 4. Styling, CSS, and JS

- Static assets live in **`wwwroot/`**:

  - **`wwwroot/css/site.css`** – your main custom stylesheet.
  - **`wwwroot/js/site.js`** – custom scripts.
  - **`wwwroot/lib/`** – client-side libraries added via LibMan (like Bootstrap/jQuery).

- **`Pages/Shared/_Layout.cshtml`** – the “master layout” (like a React `App.tsx` or HTML template).
  It includes Bootstrap, jQuery, site.css, navbar, footer, etc.
- Every page inherits this layout (see `_ViewStart.cshtml`).

If you want to modify global CSS → edit `wwwroot/css/site.css` and make sure it’s referenced in `_Layout.cshtml`.

---

## 5. Common Modifications in Web Apps

- **Add a new resource (e.g., Department)**:

  1. Create `Models/Department.cs`.
  2. Add `DbSet<Department>` in `AppDbContext`.
  3. Run `dotnet ef migrations add AddDepartment && dotnet ef database update`.
  4. Scaffold Razor Pages:

     ```bash
     dotnet aspnet-codegenerator razorpage -m Department -dc AppDbContext -udl -outDir Pages/Departments
     ```

  Boom – CRUD pages generated.

- **Change layout** → edit `_Layout.cshtml` (e.g., add sidebar, change navbar).

- **Add styling** → edit `site.css` or swap Bootstrap theme.

- **Inject services (like logging, AI clients, etc.)** → configure in `Program.cs`.

- **Handle form validation** → Razor supports DataAnnotations (e.g., `[Required]`, `[MaxLength(50)]`) on model properties → automatically validated client + server side.

---

## 6. How It Compares to JavaScript/Node.js

| Your Stack               | ASP.NET Core Equivalent                              |
| ------------------------ | ---------------------------------------------------- |
| Express.js routes        | Razor PageModels / Controllers                       |
| Sequelize/Mongoose       | Entity Framework Core (DbContext + Migrations)       |
| EJS/React SSR            | Razor (.cshtml) templates with C# expressions        |
| `server.js` + middleware | `Program.cs` + ASP.NET Core Middleware pipeline      |
| Nginx static serving     | Built-in Kestrel + `wwwroot/` static file middleware |
| `package.json`           | `.csproj` (dependencies, build config)               |

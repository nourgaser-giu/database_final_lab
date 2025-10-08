using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HrLabApp.Data;

namespace HrLabApp.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly AppDbContext _context;

    public IndexModel(ILogger<IndexModel> logger, AppDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public int EmployeeCount { get; set; }
    public int DepartmentCount { get; set; }

    public async Task OnGetAsync()
    {
        EmployeeCount = await _context.Employees.CountAsync();
        DepartmentCount = await _context.Departments.CountAsync();
    }
}

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
        EmployeeList = await _context.Employees
            .Include(e => e.Department)
            .ToListAsync();
    }
}

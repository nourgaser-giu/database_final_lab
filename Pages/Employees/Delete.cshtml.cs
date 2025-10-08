using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
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
        var emp = await _context.Employees
            .Include(e => e.Department)
            .FirstOrDefaultAsync(e => e.Id == id);
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

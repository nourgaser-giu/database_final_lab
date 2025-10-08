using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
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
    
    public SelectList DepartmentList { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var emp = await _context.Employees.FindAsync(id);
        if (emp == null) return NotFound();
        Employee = emp;
        await LoadDepartments();
        return Page();
    }
    
    private async Task LoadDepartments()
    {
        var departments = await _context.Departments.ToListAsync();
        DepartmentList = new SelectList(departments, "Id", "Name");
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadDepartments();
            return Page();
        }

        _context.Attach(Employee).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return RedirectToPage("Index");
    }
}

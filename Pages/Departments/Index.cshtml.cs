using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HrLabApp.Data;
using HrLabApp.Models;

namespace HrLabApp.Pages.Departments
{
    public class IndexModel : PageModel
    {
        private readonly HrLabApp.Data.AppDbContext _context;

        public IndexModel(HrLabApp.Data.AppDbContext context)
        {
            _context = context;
        }

        public IList<Department> Department { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Department = await _context.Departments.ToListAsync();
        }
    }
}

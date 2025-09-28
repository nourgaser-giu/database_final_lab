using Microsoft.EntityFrameworkCore;
using HrLabApp.Models;

namespace HrLabApp.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Employee> Employees { get; set; }
}

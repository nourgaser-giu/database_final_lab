using Microsoft.EntityFrameworkCore;
using HrLabApp.Models;

namespace HrLabApp.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Department> Departments { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure the one-to-many relationship
        modelBuilder.Entity<Employee>()
            .HasOne(e => e.Department)
            .WithMany(d => d.Employees)
            .HasForeignKey(e => e.DepartmentId)
            .OnDelete(DeleteBehavior.SetNull); // When department is deleted, set employee's DepartmentId to null
            
        base.OnModelCreating(modelBuilder);
    }
}

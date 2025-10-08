namespace HrLabApp.Models;

public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Salary { get; set; }
    
    // Foreign key for Department
    public int? DepartmentId { get; set; }
    
    // Navigation property
    public virtual Department? Department { get; set; }
}

namespace HrLabApp.Models;

public class Department
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    
    // Navigation property for the one-to-many relationship
    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
namespace employeeportal.Entity.Entities;

public class EmployeeProject
{
    public int EmployeeId { get; set; }
    public int ProjectId { get; set; }

    public Employee Employee { get; set; } = null!;
    public Project Project { get; set; } = null!;
}

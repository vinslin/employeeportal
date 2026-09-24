namespace employeeportal.Entity.Entities;

public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<EmployeeProject> EmployeeProjects { get; set; } = new List<EmployeeProject>();
}

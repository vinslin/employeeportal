namespace employeeportal.Entity.Entities;

public class Project
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Summary { get; set; }
    public string? Client { get; set; }

    public ICollection<EmployeeProject> EmployeeProjects { get; set; } = new List<EmployeeProject>();
}

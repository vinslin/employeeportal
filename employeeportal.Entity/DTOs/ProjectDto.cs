namespace employeeportal.Entity.DTOs;

public class ProjectDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Summary { get; set; }
    public string? Client { get; set; }
    public List<EmployeeSummaryDto> Employees { get; set; } = new();
}

using System.ComponentModel.DataAnnotations;

namespace employeeportal.Entity.DTOs;

public class CreateProjectDto
{
    [MaxLength(200)]
    public string? Name { get; set; }

    [MaxLength(500)]
    public string? Summary { get; set; }

    [MaxLength(200)]
    public string? Client { get; set; }
}

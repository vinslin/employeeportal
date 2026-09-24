using System.ComponentModel.DataAnnotations;

namespace employeeportal.Entity.DTOs;

public class CreateEmployeeDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
}

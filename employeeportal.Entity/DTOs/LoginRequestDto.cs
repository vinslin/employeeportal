using System.ComponentModel.DataAnnotations;

namespace employeeportal.Entity.DTOs;

public class LoginRequestDto
{
    [Required]
    [MaxLength(100)]
    public string UserName { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}

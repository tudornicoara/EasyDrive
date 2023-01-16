using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class RegisterDto
{
    [Required]
    public string? Name { get; set; }
    
    [Required]
    public string? Surname { get; set; }

    [Required]
    public string? Gender { get; set; }

    [Required]
    public string? City { get; set; }

    [Required]
    public string? Country { get; set; }
    
    [Required]
    public string? Password { get; set; }
}
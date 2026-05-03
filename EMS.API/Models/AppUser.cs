using System.ComponentModel.DataAnnotations;

namespace EMS.API.Models;

/// <summary>
/// Represents an application user that can authenticate into the EMS dashboard.
/// </summary>
public class AppUser
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string Role { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}

using System.ComponentModel.DataAnnotations;
using Orchestra.IAM.Domain.Enums;

namespace Orchestra.IAM.Domain.Entities;

public class AppUser
{
    public Guid Id { get; set; }
    [MaxLength(256)]
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public Role UserRole { get; set; } = Role.User;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
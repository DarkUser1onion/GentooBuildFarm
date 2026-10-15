using Microsoft.AspNetCore.Identity;

namespace GentooBuildFarm.Models;

public class AppUser : IdentityUser
{
    public string? Nickname { get; set; }
    public string? PgpFingerprint { get; set; }
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
}
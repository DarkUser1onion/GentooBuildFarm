namespace GentooBuildFarm.Models;

/// <summary>M:N связка User <-> Package. Составной PK.</summary>
public class Subscription
{
    public string UserId { get; set; } = string.Empty;
    public AppUser User { get; set; } = null!;

    public int PackageId { get; set; }
    public Package Package { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
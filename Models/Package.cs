using System.ComponentModel.DataAnnotations;

namespace GentooBuildFarm.Models;

public class Package
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Category { get; set; } = string.Empty;

    [Required]
    [StringLength(30)]
    public string Version { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [StringLength(200)]
    public string? Homepage { get; set; }

    public string AddedById { get; set; } = string.Empty;
    public AppUser? AddedBy { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Навигация
    public List<PackageUseFlag> UseFlags { get; set; } = new();
    public List<BuildTask> BuildTasks { get; set; } = new();
    public List<Subscription> Subscriptions { get; set; } = new();
}
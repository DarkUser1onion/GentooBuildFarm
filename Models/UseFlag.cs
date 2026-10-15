using System.ComponentModel.DataAnnotations;

namespace GentooBuildFarm.Models;

public class UseFlag
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;

    [StringLength(200)]
    public string? Description { get; set; }

    public List<PackageUseFlag> Packages { get; set; } = new();
}
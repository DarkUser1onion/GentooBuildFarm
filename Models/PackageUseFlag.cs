namespace GentooBuildFarm.Models;

/// <summary>M:N связка Package <-> UseFlag. Тут составной PK.</summary>
public class PackageUseFlag
{
    public int PackageId { get; set; }
    public Package Package { get; set; } = null!;

    public int UseFlagId { get; set; }
    public UseFlag UseFlag { get; set; } = null!;
}
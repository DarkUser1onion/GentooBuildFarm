using System.ComponentModel.DataAnnotations;

namespace GentooBuildFarm.Models;

public enum BuildStatus { Queued, Building, Success, Failed }
public enum BuildPriority { Low, Normal, High }

public class BuildTask
{
    public int Id { get; set; }

    public int PackageId { get; set; }
    public Package Package { get; set; } = null!;

    public BuildStatus Status { get; set; } = BuildStatus.Queued;
    public BuildPriority Priority { get; set; } = BuildPriority.Normal;

    public DateTime? StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }

    public string TriggeredById { get; set; } = string.Empty;
    public AppUser? TriggeredBy { get; set; }

    public int? WorkerId { get; set; }
    public Worker? Worker { get; set; }

    public string? LogContent { get; set; }
    public int? ExitCode { get; set; }
}
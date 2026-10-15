using System.ComponentModel.DataAnnotations;

namespace GentooBuildFarm.Models;

public enum WorkerStatus { Idle, Busy, Offline }

public class Worker
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string Architecture { get; set; } = string.Empty;

    public WorkerStatus Status { get; set; } = WorkerStatus.Idle;

    public DateTime LastHeartbeat { get; set; } = DateTime.UtcNow;

    public List<BuildTask> BuildTasks { get; set; } = new();
}
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GentooBuildFarm.Models;

namespace GentooBuildFarm.Data;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Package> Packages => Set<Package>();
    public DbSet<UseFlag> UseFlags => Set<UseFlag>();
    public DbSet<PackageUseFlag> PackageUseFlags => Set<PackageUseFlag>();
    public DbSet<Worker> Workers => Set<Worker>();
    public DbSet<BuildTask> BuildTasks => Set<BuildTask>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);

        b.Entity<BuildTask>().HasIndex(x => x.Status);
        b.Entity<BuildTask>().HasIndex(x => x.StartedAt);
        b.Entity<Package>().HasIndex(x => x.CreatedAt);
        b.Entity<Worker>().HasIndex(x => x.LastHeartbeat);

        b.Entity<PackageUseFlag>(e =>
        {
            e.HasKey(x => new { x.PackageId, x.UseFlagId });

            e.HasOne(x => x.Package)
             .WithMany(p => p.UseFlags)
             .HasForeignKey(x => x.PackageId)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(x => x.UseFlag)
             .WithMany(u => u.Packages)
             .HasForeignKey(x => x.UseFlagId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        b.Entity<Subscription>(e =>
        {
            e.HasKey(x => new { x.UserId, x.PackageId });

            e.HasOne(x => x.User)
             .WithMany()
             .HasForeignKey(x => x.UserId)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(x => x.Package)
             .WithMany(p => p.Subscriptions)
             .HasForeignKey(x => x.PackageId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        b.Entity<BuildTask>()
            .HasOne(x => x.Package)
            .WithMany(p => p.BuildTasks)
            .HasForeignKey(x => x.PackageId)
            .OnDelete(DeleteBehavior.Cascade);

        b.Entity<BuildTask>()
            .HasOne(x => x.Worker)
            .WithMany(w => w.BuildTasks)
            .HasForeignKey(x => x.WorkerId)
            .OnDelete(DeleteBehavior.SetNull);

        b.Entity<Package>()
            .HasOne(x => x.AddedBy)
            .WithMany()
            .HasForeignKey(x => x.AddedById)
            .OnDelete(DeleteBehavior.Restrict);

        b.Entity<BuildTask>()
            .HasOne(x => x.TriggeredBy)
            .WithMany()
            .HasForeignKey(x => x.TriggeredById)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
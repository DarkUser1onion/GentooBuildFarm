using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using GentooBuildFarm.Data;
using GentooBuildFarm.Models;

namespace GentooBuildFarm.Services;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        var db = services.GetRequiredService<AppDbContext>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<AppUser>>();

        await db.Database.MigrateAsync();

        string[] roles = { "Admin", "Maintainer", "Builder", "User" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        var admin = await EnsureUser(userManager, "admin@buildfarm.local", "Admin123!", "admin", "Admin");
        var mike  = await EnsureUser(userManager, "mike@buildfarm.local",  "Mike123!",  "mike",  "Maintainer");
        var bob   = await EnsureUser(userManager, "bob@buildfarm.local",   "Bob123!",   "bob",   "Builder");

        if (await db.Packages.AnyAsync()) return;

        var flags = new[]
        {
            new UseFlag { Name = "systemd",    Description = "Использовать systemd" },
            new UseFlag { Name = "wayland",    Description = "Поддержка Wayland" },
            new UseFlag { Name = "pulseaudio", Description = "Поддержка PulseAudio" },
            new UseFlag { Name = "X",          Description = "Поддержка X11" },
            new UseFlag { Name = "gtk",        Description = "GTK-интерфейс" },
            new UseFlag { Name = "qt5",        Description = "Qt5-интерфейс" },
            new UseFlag { Name = "minimal",    Description = "Минимальная сборка" },
            new UseFlag { Name = "debug",      Description = "Отладочные символы" },
        };
        db.UseFlags.AddRange(flags);
        await db.SaveChangesAsync();

        var packages = new[]
        {
            new Package { Name = "firefox",    Category = "www-client",    Version = "128.0",  Description = "Firefox web browser", Homepage = "https://mozilla.org",     AddedById = mike.Id },
            new Package { Name = "neovim",     Category = "app-editors",   Version = "0.10.1", Description = "Vim-fork focused on extensibility", Homepage = "https://neovim.io", AddedById = mike.Id },
            new Package { Name = "rust",       Category = "dev-lang",      Version = "1.80.0", Description = "Rust programming language", Homepage = "https://rust-lang.org", AddedById = mike.Id },
            new Package { Name = "python",     Category = "dev-lang",      Version = "3.12.0", Description = "Python interpreter", Homepage = "https://python.org", AddedById = admin.Id },
            new Package { Name = "postgresql", Category = "dev-db",        Version = "16.3",   Description = "PostgreSQL database", Homepage = "https://postgresql.org", AddedById = admin.Id },
        };
        db.Packages.AddRange(packages);
        await db.SaveChangesAsync();

        var links = new List<PackageUseFlag>
        {
            new() { PackageId = packages[0].Id, UseFlagId = flags[0].Id }, // firefox + systemd
            new() { PackageId = packages[0].Id, UseFlagId = flags[1].Id }, // firefox + wayland
            new() { PackageId = packages[0].Id, UseFlagId = flags[2].Id }, // firefox + pulseaudio
            new() { PackageId = packages[0].Id, UseFlagId = flags[4].Id }, // firefox + gtk
            new() { PackageId = packages[1].Id, UseFlagId = flags[0].Id }, // neovim + systemd
            new() { PackageId = packages[1].Id, UseFlagId = flags[7].Id }, // neovim + debug
            new() { PackageId = packages[2].Id, UseFlagId = flags[0].Id }, // rust + systemd
            new() { PackageId = packages[3].Id, UseFlagId = flags[0].Id }, // python + systemd
            new() { PackageId = packages[3].Id, UseFlagId = flags[6].Id }, // python + minimal
            new() { PackageId = packages[4].Id, UseFlagId = flags[0].Id }, // postgresql + systemd
        };
        db.PackageUseFlags.AddRange(links);

        var workers = new[]
        {
            new Worker { Name = "build-node-01", Architecture = "amd64", Status = WorkerStatus.Idle },
            new Worker { Name = "build-node-02", Architecture = "amd64", Status = WorkerStatus.Idle },
            new Worker { Name = "build-node-arm", Architecture = "arm64", Status = WorkerStatus.Idle },
        };
        db.Workers.AddRange(workers);
        await db.SaveChangesAsync();

        var tasks = new List<BuildTask>
        {
            new() { PackageId = packages[0].Id, Status = BuildStatus.Success,  Priority = BuildPriority.Normal, StartedAt = DateTime.UtcNow.AddDays(-2), FinishedAt = DateTime.UtcNow.AddDays(-2).AddHours(2), TriggeredById = mike.Id,  WorkerId = workers[0].Id, ExitCode = 0, LogContent = ">>> emerge success" },
            new() { PackageId = packages[1].Id, Status = BuildStatus.Success,  Priority = BuildPriority.Low,    StartedAt = DateTime.UtcNow.AddDays(-1), FinishedAt = DateTime.UtcNow.AddDays(-1).AddMinutes(30), TriggeredById = mike.Id,  WorkerId = workers[0].Id, ExitCode = 0, LogContent = ">>> emerge success" },
            new() { PackageId = packages[2].Id, Status = BuildStatus.Failed,   Priority = BuildPriority.High,   StartedAt = DateTime.UtcNow.AddHours(-5), FinishedAt = DateTime.UtcNow.AddHours(-3), TriggeredById = admin.Id, WorkerId = workers[1].Id, ExitCode = 1, LogContent = ">>> emerge failed: unresolved dependency" },
            new() { PackageId = packages[3].Id, Status = BuildStatus.Queued,   Priority = BuildPriority.Normal, TriggeredById = admin.Id },
            new() { PackageId = packages[4].Id, Status = BuildStatus.Queued,   Priority = BuildPriority.High,   TriggeredById = admin.Id },
        };
        db.BuildTasks.AddRange(tasks);

        await db.SaveChangesAsync();
    }

    private static async Task<AppUser> EnsureUser(
        UserManager<AppUser> userManager,
        string email, string password, string nickname, string role)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            user = new AppUser
            {
                UserName = email,
                Email = email,
                Nickname = nickname,
                EmailConfirmed = true
            };
            await userManager.CreateAsync(user, password);
            await userManager.AddToRoleAsync(user, role);
        }
        return user;
    }
}
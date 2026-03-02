using Microsoft.EntityFrameworkCore;
using RealmAuthApi.Models;
using RealmAuthApi.Security;

namespace RealmAuthApi.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(
        RealmAuthDbContext db,
        IPasswordHasher passwordHasher,
        CancellationToken ct = default)
    {
        // Ensure schema is applied (safe if already applied)
        await db.Database.MigrateAsync(ct);

        const string username = "testname";
        const string email = "testname@example.com";
        const string password = "password";

        var account = await db.Accounts
            .Include(a => a.Characters)
            .FirstOrDefaultAsync(a => a.Username == username, ct);

        if (account is null)
        {
            account = new Account
            {
                Username = username,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                CreatedAt = DateTimeOffset.UtcNow
            };

            db.Accounts.Add(account);
            await db.SaveChangesAsync(ct);
        }

        await EnsureCharacterAsync(db, account.Id, "Test Templar", "templar", 1, ct);
        await EnsureCharacterAsync(db, account.Id, "Test Ranger", "ranger", 5, ct);

        await EnsureMapsAsync(db, ct);

        await db.SaveChangesAsync(ct);
    }

    private static async Task EnsureCharacterAsync(
        RealmAuthDbContext db,
        int accountId,
        string name,
        string classId,
        int level,
        CancellationToken ct)
    {
        var exists = await db.Characters.AnyAsync(
            c => c.AccountId == accountId && c.Name == name, ct);

        if (exists)
            return;

        db.Characters.Add(new Character
        {
            AccountId = accountId,
            Name = name,
            ClassId = classId,
            Level = level,
            CreatedAt = DateTimeOffset.UtcNow
        });
    }

    private static async Task EnsureMapsAsync(RealmAuthDbContext db, CancellationToken ct)
{
    // Upsert-ish helper by slug
    async Task<Map> GetOrCreateAsync(
        string slug,
        string displayName,
        string scenePath,
        string kind,
        bool isPlayable,
        bool isHidden,
        string[] tags,
        int sortOrder,
        int? minLevel = null,
        int? maxLevel = null)
    {
        var m = await db.Maps.FirstOrDefaultAsync(x => x.Slug == slug, ct);
        if (m is not null)
            return m;

        m = new Map
        {
            Slug = slug,
            DisplayName = displayName,
            ScenePath = scenePath,
            Kind = kind,
            IsPlayable = isPlayable,
            IsHidden = isHidden,
            Tags = tags,
            SortOrder = sortOrder,
            MinLevel = minLevel,
            MaxLevel = maxLevel,
            CreatedAt = DateTimeOffset.UtcNow
        };

        db.Maps.Add(m);
        await db.SaveChangesAsync(ct); // so it gets an Id for links
        return m;
    }

    var hub = await GetOrCreateAsync(
        slug: "hub",
        displayName: "Hub",
        scenePath: "res://maps/HubMap.tscn",
        kind: "hub",
        isPlayable: true,
        isHidden: false,
        tags: new[] { "hub", "safe", "vendor" },
        sortOrder: 0);

    var forest = await GetOrCreateAsync(
        slug: "act1_forest_01",
        displayName: "Whispering Forest",
        scenePath: "res://maps/DebugMap.tscn",
        kind: "overworld",
        isPlayable: true,
        isHidden: false,
        tags: new[] { "act1", "forest", "combat" },
        sortOrder: 10,
        minLevel: 1,
        maxLevel: 10);

    // Links (avoid duplicates via unique index)
    async Task EnsureLinkAsync(Map from, Map to, string kind, string label, int sortOrder)
    {
        var exists = await db.MapLinks.AnyAsync(l =>
            l.FromMapId == from.Id &&
            l.ToMapId == to.Id &&
            l.LinkKind == kind &&
            l.Label == label, ct);

        if (exists) return;

        db.MapLinks.Add(new MapLink
        {
            FromMapId = from.Id,
            ToMapId = to.Id,
            LinkKind = kind,
            Label = label,
            SortOrder = sortOrder,
            IsEnabled = true,
            CreatedAt = DateTimeOffset.UtcNow
        });
    }

    await EnsureLinkAsync(hub, forest, "portal", "To Whispering Forest", 0);
    await EnsureLinkAsync(forest, hub, "portal", "Return to Hub", 0);
}

}
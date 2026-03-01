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
}
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealmAuthApi.Data;
using RealmAuthApi.Models;

namespace RealmAuthApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class CharactersController : ControllerBase
{
    private readonly RealmAuthDbContext _db;
    public CharactersController(RealmAuthDbContext db) => _db = db;

    private int AccountId()
    {
        var uid = User.FindFirstValue("uid") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(uid, out var id)) throw new UnauthorizedAccessException("Missing uid claim.");
        return id;
    }

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var accountId = AccountId();
        var chars = await _db.Characters
            .Where(c => c.AccountId == accountId)
            .OrderBy(c => c.Id)
            .Select(c => new { id = c.Id, name = c.Name, classId = c.ClassId, level = c.Level })
            .ToListAsync();

        return Ok(new { characters = chars });
    }

    public sealed record CreateRequest(string Name, string? ClassId);

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRequest req)
    {
        var accountId = AccountId();

        var name = (req.Name ?? "").Trim();
        if (name.Length < 3 || name.Length > 32) return BadRequest(new { error = "Name must be 3–32 chars." });

        // Optional: cap characters per account
        var count = await _db.Characters.CountAsync(c => c.AccountId == accountId);
        if (count >= 10) return Conflict(new { error = "Character limit reached." });

        var classId = (req.ClassId ?? "templar").Trim().ToLowerInvariant();
        if (classId.Length < 2 || classId.Length > 32) return BadRequest(new { error = "Invalid classId." });

        var exists = await _db.Characters.AnyAsync(c => c.AccountId == accountId && c.Name == name);
        if (exists) return Conflict(new { error = "Character name already used on this account." });

        var ch = new Character
        {
            AccountId = accountId,
            Name = name,
            ClassId = classId,
            Level = 1,
            CreatedAt = DateTimeOffset.UtcNow
        };

        _db.Characters.Add(ch);
        await _db.SaveChangesAsync();

        return Ok(new { id = ch.Id, name = ch.Name, classId = ch.ClassId, level = ch.Level });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        var accountId = AccountId();
        var ch = await _db.Characters.FirstOrDefaultAsync(c => c.Id == id && c.AccountId == accountId);
        if (ch is null) return NotFound(new { error = "Not found." });

        _db.Characters.Remove(ch);
        await _db.SaveChangesAsync();
        return Ok(new { ok = true });
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealmAuthApi.Data;

namespace RealmAuthApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous] // If you want maps public, change to [AllowAnonymous]
public sealed class MapsController : ControllerBase
{
    private readonly RealmAuthDbContext _db;
    public MapsController(RealmAuthDbContext db) => _db = db;

    // GET /api/maps?playable=true&hidden=false&kind=hub&tag=act1&search=forest
    [HttpGet]
    public async Task<IActionResult> List(
        [FromQuery] bool? playable,
        [FromQuery] bool? hidden,
        [FromQuery] string? kind,
        [FromQuery] string? tag,
        [FromQuery] string? search)
    {
        var q = _db.Maps.AsQueryable();

        if (playable is not null) q = q.Where(m => m.IsPlayable == playable.Value);
        if (hidden is not null) q = q.Where(m => m.IsHidden == hidden.Value);

        if (!string.IsNullOrWhiteSpace(kind))
        {
            var k = kind.Trim().ToLowerInvariant();
            q = q.Where(m => m.Kind == k);
        }

        if (!string.IsNullOrWhiteSpace(tag))
        {
            var t = tag.Trim().ToLowerInvariant();
            // Postgres array contains
            q = q.Where(m => m.Tags.Contains(t));
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            q = q.Where(m =>
                EF.Functions.ILike(m.DisplayName, $"%{s}%") ||
                EF.Functions.ILike(m.Slug, $"%{s}%"));
        }

        var maps = await q
            .OrderBy(m => m.SortOrder)
            .ThenBy(m => m.Id)
            .Select(m => new
            {
                id = m.Id,
                slug = m.Slug,
                name = m.DisplayName,
                scenePath = m.ScenePath,
                kind = m.Kind,
                playable = m.IsPlayable,
                hidden = m.IsHidden,
                tags = m.Tags,
                minLevel = m.MinLevel,
                maxLevel = m.MaxLevel
            })
            .ToListAsync();

        return Ok(new { maps });
    }

    // GET /api/maps/123
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get([FromRoute] int id)
    {
        var m = await _db.Maps
            .Where(x => x.Id == id)
            .Select(x => new
            {
                id = x.Id,
                slug = x.Slug,
                name = x.DisplayName,
                scenePath = x.ScenePath,
                kind = x.Kind,
                playable = x.IsPlayable,
                hidden = x.IsHidden,
                tags = x.Tags,
                minLevel = x.MinLevel,
                maxLevel = x.MaxLevel,
                sortOrder = x.SortOrder
            })
            .FirstOrDefaultAsync();

        if (m is null) return NotFound(new { error = "Not found." });
        return Ok(m);
    }

    // GET /api/maps/123/links
    [HttpGet("{id:int}/links")]
    public async Task<IActionResult> Links([FromRoute] int id)
    {
        var exists = await _db.Maps.AnyAsync(m => m.Id == id);
        if (!exists) return NotFound(new { error = "Map not found." });

        var links = await _db.MapLinks
            .Where(l => l.FromMapId == id && l.IsEnabled)
            .OrderBy(l => l.SortOrder)
            .ThenBy(l => l.Id)
            .Select(l => new
            {
                id = l.Id,
                kind = l.LinkKind,
                label = l.Label,
                to = new
                {
                    id = l.ToMap.Id,
                    slug = l.ToMap.Slug,
                    name = l.ToMap.DisplayName,
                    scenePath = l.ToMap.ScenePath,
                    kind = l.ToMap.Kind,
                    playable = l.ToMap.IsPlayable,
                    hidden = l.ToMap.IsHidden,
                    tags = l.ToMap.Tags,
                    minLevel = l.ToMap.MinLevel,
                    maxLevel = l.ToMap.MaxLevel
                }
            })
            .ToListAsync();

        return Ok(new { links });
    }

    // GET /api/maps/123/spawns?tag=default
    [HttpGet("{id:int}/spawns")]
    public async Task<IActionResult> Spawns([FromRoute] int id, [FromQuery] string? tag)
    {
        var exists = await _db.Maps.AnyAsync(m => m.Id == id);
        if (!exists) return NotFound(new { error = "Map not found." });

        var q = _db.MapSpawnEntries.Where(s => s.MapId == id);

        // Optional filtering if you want variants like ?tag=nightmare
        if (!string.IsNullOrWhiteSpace(tag))
        {
            var t = tag.Trim().ToLowerInvariant();
            q = q.Where(s => s.Tags.Contains(t));
        }

        var spawns = await q
            .OrderByDescending(s => s.Weight)
            .ThenBy(s => s.TypeId)
            .Select(s => new
            {
                id = s.Id,
                typeId = s.TypeId,
                weight = s.Weight,
                minPackSize = s.MinPackSize,
                maxPackSize = s.MaxPackSize,
                minPacks = s.MinPacks,
                maxPacks = s.MaxPacks,
                minLevel = s.MinLevel,
                maxLevel = s.MaxLevel,
                tags = s.Tags
            })
            .ToListAsync();

        return Ok(new { mapId = id, spawns });
    }
}
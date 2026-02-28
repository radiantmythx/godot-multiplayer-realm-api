using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealmAuthApi.Data;

namespace RealmAuthApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class HealthController : ControllerBase
{
    private readonly RealmAuthDbContext _db;

    public HealthController(RealmAuthDbContext db) => _db = db;

    [HttpGet("db")]
    public async Task<IActionResult> Db()
    {
        var canConnect = await _db.Database.CanConnectAsync();
        return Ok(new { ok = canConnect });
    }
}
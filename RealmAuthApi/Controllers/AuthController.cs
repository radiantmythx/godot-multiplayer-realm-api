using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealmAuthApi.Data;
using RealmAuthApi.Models;
using RealmAuthApi.Security;

namespace RealmAuthApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly RealmAuthDbContext _db;
    private readonly JwtTokenService _tokens;
    private readonly IPasswordHasher _hasher;

    public AuthController(RealmAuthDbContext db, JwtTokenService tokens, IPasswordHasher hasher)
    {
        _db = db;
        _tokens = tokens;
        _hasher = hasher;
    }

    public sealed record RegisterRequest(string Username, string Email, string Password);
    public sealed record LoginRequest(string UsernameOrEmail, string Password);

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest req)
    {
        var username = (req.Username ?? "").Trim();
        var email = (req.Email ?? "").Trim().ToLowerInvariant();

        if (username.Length < 3 || username.Length > 32)
            return BadRequest(new { error = "Username must be 3-32 characters." });

        if (email.Length < 5 || email.Length > 255 || !email.Contains('@'))
            return BadRequest(new { error = "Email looks invalid." });

        if ((req.Password ?? "").Length < 8)
            return BadRequest(new { error = "Password must be at least 8 characters." });

        var exists = await _db.Accounts.AnyAsync(a => a.Username == username || a.Email == email);
        if (exists)
            return Conflict(new { error = "Username or email already exists." });

        var hash = _hasher.Hash(req.Password ?? "");

        var account = new Account
        {
            Username = username,
            Email = email,
            PasswordHash = hash,
            CreatedAt = DateTimeOffset.UtcNow
        };

        _db.Accounts.Add(account);
        await _db.SaveChangesAsync();

        // Optional: auto-login on register
        var token = _tokens.CreateToken(account.Id, account.Username);

        return Ok(new
        {
            accountId = account.Id,
            username = account.Username,
            token
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        var key = (req.UsernameOrEmail ?? "").Trim();
        var isEmail = key.Contains('@');
        var normalizedEmail = key.ToLowerInvariant();

        var account = await _db.Accounts.FirstOrDefaultAsync(a =>
            isEmail ? a.Email == normalizedEmail : a.Username == key);

        if (account is null)
            return Unauthorized(new { error = "Invalid credentials." });

        var ok = _hasher.Verify(account.PasswordHash, req.Password ?? "");
        if (!ok)
            return Unauthorized(new { error = "Invalid credentials." });

        var token = _tokens.CreateToken(account.Id, account.Username);

        return Ok(new
        {
            accountId = account.Id,
            username = account.Username,
            token
        });
    }
}
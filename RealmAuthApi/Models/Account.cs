namespace RealmAuthApi.Models;

public sealed class Account
{
    public int Id { get; set; }

    public string Username { get; set; } = "";
    public string Email { get; set; } = "";
    public string PasswordHash { get; set; } = "";

    public List<Character> Characters { get; set; } = new();

    public DateTimeOffset CreatedAt { get; set; }
}
namespace RealmAuthApi.Data;

public sealed class Account
{
    public int Id { get; set; }

    public string Username { get; set; } = "";
    public string Email { get; set; } = "";
    public string PasswordHash { get; set; } = "";

    public DateTimeOffset CreatedAt { get; set; }
}
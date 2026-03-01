namespace RealmAuthApi.Models;

public sealed class Character
{
    public int Id { get; set; }

    public int AccountId { get; set; }
    public Account Account { get; set; } = null!;

    public string Name { get; set; } = "";
    public string ClassId { get; set; } = "templar"; // optional, whatever you want
    public int Level { get; set; } = 1;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
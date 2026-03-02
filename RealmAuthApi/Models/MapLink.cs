namespace RealmAuthApi.Models;

public sealed class MapLink
{
    public int Id { get; set; }

    public int FromMapId { get; set; }
    public Map FromMap { get; set; } = null!;

    public int ToMapId { get; set; }
    public Map ToMap { get; set; } = null!;

    // waypoint / portal / door / npc / dev_teleport
    public string LinkKind { get; set; } = "portal";

    // UI label like "To The Forest"
    public string Label { get; set; } = "";

    public bool IsEnabled { get; set; } = true;

    public int SortOrder { get; set; } = 0;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
namespace RealmAuthApi.Models;

public sealed class Map
{
    public int Id { get; set; }

    // Stable human-friendly identifier (great for logs/debug/admin)
    public string Slug { get; set; } = "";

    // What shows in UI
    public string DisplayName { get; set; } = "";

    // Godot scene path, e.g. res://zones/hub/HubMap.tscn
    public string ScenePath { get; set; } = "";

    // hub / overworld / dungeon / arena / test, etc.
    public string Kind { get; set; } = "overworld";

    public bool IsPlayable { get; set; } = true;
    public bool IsHidden { get; set; } = false;

    // Postgres text[] (via Npgsql)
    public string[] Tags { get; set; } = [];

    public int? MinLevel { get; set; }
    public int? MaxLevel { get; set; }

    public int SortOrder { get; set; } = 0;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    // navigation
    public List<MapLink> OutgoingLinks { get; set; } = new();
    public List<MapLink> IncomingLinks { get; set; } = new();
}
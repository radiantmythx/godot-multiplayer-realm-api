namespace RealmAuthApi.Models;

public sealed class MapSpawnEntry
{
    public int Id { get; set; }

    public int MapId { get; set; }
    public Map Map { get; set; } = null!;

    // Must exist in Godot MonsterDatabase (type_id)
    public string TypeId { get; set; } = "";

    // Weighted random selection within a spawn pool
    public float Weight { get; set; } = 1f;

    // Pack sizing knobs (you can interpret these however you want in Zone)
    public int MinPackSize { get; set; } = 3;
    public int MaxPackSize { get; set; } = 6;

    // Optional: how many packs to spawn per “spawn point” / per map, etc.
    public int MinPacks { get; set; } = 1;
    public int MaxPacks { get; set; } = 1;

    // Optional knobs for future
    public int? MinLevel { get; set; }
    public int? MaxLevel { get; set; }

    // text[] tags for internal grouping like ["default","day","rare"]
    public string[] Tags { get; set; } = [];
}
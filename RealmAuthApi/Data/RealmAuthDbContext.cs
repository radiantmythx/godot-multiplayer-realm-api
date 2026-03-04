using Microsoft.EntityFrameworkCore;
using RealmAuthApi.Models;

namespace RealmAuthApi.Data;

public sealed class RealmAuthDbContext : DbContext
{
    public RealmAuthDbContext(DbContextOptions options) : base(options) {}

    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Character> Characters => Set<Character>();
    public DbSet<Map> Maps => Set<Map>();
    public DbSet<MapLink> MapLinks => Set<MapLink>();
    public DbSet<MapSpawnEntry> MapSpawnEntries => Set<MapSpawnEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Account>(e =>
        {
            e.ToTable("accounts");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.Username).HasColumnName("username").HasMaxLength(32).IsRequired();
            e.Property(x => x.Email).HasColumnName("email").HasMaxLength(255).IsRequired();
            e.Property(x => x.PasswordHash).HasColumnName("password_hash").IsRequired();
            e.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
            e.HasIndex(x => x.Username).IsUnique();
            e.HasIndex(x => x.Email).IsUnique();
        });

        modelBuilder.Entity<Character>(e =>
        {
            e.ToTable("characters");
            e.HasKey(x => x.Id);

            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.AccountId).HasColumnName("account_id").IsRequired();
            e.Property(x => x.Name).HasColumnName("name").HasMaxLength(32).IsRequired();
            e.Property(x => x.ClassId).HasColumnName("class_id").HasMaxLength(32).IsRequired();
            e.Property(x => x.Level).HasColumnName("level").IsRequired();
            e.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");

            e.HasIndex(x => new { x.AccountId, x.Name }).IsUnique();

            e.HasOne(x => x.Account)
                .WithMany(a => a.Characters)
                .HasForeignKey(x => x.AccountId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        modelBuilder.Entity<Map>(e =>
        {
            e.ToTable("maps");
            e.HasKey(x => x.Id);

            e.Property(x => x.Id).HasColumnName("id");

            e.Property(x => x.Slug)
                .HasColumnName("slug")
                .HasMaxLength(64)
                .IsRequired();

            e.Property(x => x.DisplayName)
                .HasColumnName("display_name")
                .HasMaxLength(128)
                .IsRequired();

            e.Property(x => x.ScenePath)
                .HasColumnName("scene_path")
                .HasMaxLength(255)
                .IsRequired();

            e.Property(x => x.Kind)
                .HasColumnName("kind")
                .HasMaxLength(32)
                .IsRequired();

            e.Property(x => x.IsPlayable).HasColumnName("is_playable").IsRequired();
            e.Property(x => x.IsHidden).HasColumnName("is_hidden").IsRequired();

            // Npgsql maps string[] to Postgres text[]
            e.Property(x => x.Tags)
                .HasColumnName("tags")
                .HasColumnType("text[]")
                .IsRequired();

            e.Property(x => x.MinLevel).HasColumnName("min_level");
            e.Property(x => x.MaxLevel).HasColumnName("max_level");

            e.Property(x => x.SortOrder).HasColumnName("sort_order").IsRequired();

            e.Property(x => x.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("NOW()");

            e.HasIndex(x => x.Slug).IsUnique();
            e.HasIndex(x => new { x.IsPlayable, x.IsHidden, x.SortOrder });
        });

        modelBuilder.Entity<MapLink>(e =>
        {
            e.ToTable("map_links");
            e.HasKey(x => x.Id);

            e.Property(x => x.Id).HasColumnName("id");

            e.Property(x => x.FromMapId).HasColumnName("from_map_id").IsRequired();
            e.Property(x => x.ToMapId).HasColumnName("to_map_id").IsRequired();

            e.Property(x => x.LinkKind)
                .HasColumnName("link_kind")
                .HasMaxLength(32)
                .IsRequired();

            e.Property(x => x.Label)
                .HasColumnName("label")
                .HasMaxLength(128)
                .IsRequired();

            e.Property(x => x.IsEnabled).HasColumnName("is_enabled").IsRequired();
            e.Property(x => x.SortOrder).HasColumnName("sort_order").IsRequired();

            e.Property(x => x.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("NOW()");

            // Relationships (no cascade delete loops)
            e.HasOne(x => x.FromMap)
                .WithMany(m => m.OutgoingLinks)
                .HasForeignKey(x => x.FromMapId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.ToMap)
                .WithMany(m => m.IncomingLinks)
                .HasForeignKey(x => x.ToMapId)
                .OnDelete(DeleteBehavior.Restrict);

            // Avoid duplicate edges of same kind/label if you want:
            e.HasIndex(x => new { x.FromMapId, x.ToMapId, x.LinkKind, x.Label }).IsUnique();

            e.HasIndex(x => new { x.FromMapId, x.IsEnabled, x.SortOrder });
        });

        modelBuilder.Entity<MapSpawnEntry>(e =>
        {
            e.ToTable("map_spawn_entries");
            e.HasKey(x => x.Id);

            e.Property(x => x.Id).HasColumnName("id");

            e.Property(x => x.MapId).HasColumnName("map_id").IsRequired();

            e.Property(x => x.TypeId)
                .HasColumnName("type_id")
                .HasMaxLength(64)
                .IsRequired();

            e.Property(x => x.Weight).HasColumnName("weight").IsRequired();

            e.Property(x => x.MinPackSize).HasColumnName("min_pack_size").IsRequired();
            e.Property(x => x.MaxPackSize).HasColumnName("max_pack_size").IsRequired();

            e.Property(x => x.MinPacks).HasColumnName("min_packs").IsRequired();
            e.Property(x => x.MaxPacks).HasColumnName("max_packs").IsRequired();

            e.Property(x => x.MinLevel).HasColumnName("min_level");
            e.Property(x => x.MaxLevel).HasColumnName("max_level");

            e.Property(x => x.Tags)
                .HasColumnName("tags")
                .HasColumnType("text[]")
                .IsRequired();

            e.HasOne(x => x.Map)
                .WithMany()
                .HasForeignKey(x => x.MapId)
                .OnDelete(DeleteBehavior.Cascade);

            // Prevent duplicate entries per map/type within same tag-set (simple version)
            e.HasIndex(x => new { x.MapId, x.TypeId }).IsUnique();

            e.HasIndex(x => new { x.MapId, x.Weight });
        });

    }
}
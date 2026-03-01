using Microsoft.EntityFrameworkCore;
using RealmAuthApi.Models;

namespace RealmAuthApi.Data;

public sealed class RealmAuthDbContext : DbContext
{
    public RealmAuthDbContext(DbContextOptions options) : base(options) {}

    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Character> Characters => Set<Character>();

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
    }
}
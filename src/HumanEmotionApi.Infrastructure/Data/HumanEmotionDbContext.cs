using HumanEmotionApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HumanEmotionApi.Infrastructure.Data;

public class HumanEmotionDbContext : DbContext
{
    public HumanEmotionDbContext(DbContextOptions<HumanEmotionDbContext> options)
        : base(options)
    {
    }

    public DbSet<Human> Humans => Set<Human>();
    public DbSet<Emotion> Emotions => Set<Emotion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Human>(builder =>
        {
            builder.ToTable("Humans");
            builder.HasKey(h => h.Id);
            builder.Property(h => h.Id).ValueGeneratedNever();

            builder.Property(h => h.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(h => h.CreatedAtUtc)
                .IsRequired();

            builder.Property(h => h.UpdatedAtUtc);

            // Computed domain properties - ignore in database mapping
            builder.Ignore(h => h.ShowColor);
            builder.Ignore(h => h.EmotionScorePercentage);

            // Configure navigation to use backing field _emotions
            builder.Navigation(h => h.Emotions)
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.HasMany(h => h.Emotions)
                .WithOne()
                .HasForeignKey(e => e.HumanId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Emotion>(builder =>
        {
            builder.ToTable("Emotions");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).ValueGeneratedNever();

            builder.Property(e => e.HumanId)
                .IsRequired();

            builder.Property(e => e.Type)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(e => e.Percentage)
                .IsRequired();

            builder.Property(e => e.CreatedAtUtc)
                .IsRequired();

            builder.Property(e => e.UpdatedAtUtc);

            // Each basic emotion type can be present at most once per human
            builder.HasIndex(e => new { e.HumanId, e.Type })
                .IsUnique();
        });
    }
}

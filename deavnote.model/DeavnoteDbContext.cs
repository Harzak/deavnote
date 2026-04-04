using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace deavnote.model;

public sealed class DeavnoteDbContext : DbContext
{
    public DeavnoteDbContext(DbContextOptions<DeavnoteDbContext> options) : base(options)
    {
        ArgumentNullException.ThrowIfNull(options);
    }

    public DbSet<DevTask> Tasks { get; set; } = null!;
    public DbSet<TimeEntry> TimeEntries { get; set; } = null!;
    public DbSet<Todo> Todos { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureTask(modelBuilder);
        ConfigureTimeEntry(modelBuilder);
        ConfigureTodo(modelBuilder);
    }

    private static void ConfigureTask(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DevTask>(entity =>
        {
            entity.ToTable("Tasks");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Code)
                  .IsRequired()
                  .HasMaxLength(50);

            entity.Property(e => e.Name)
                  .IsRequired()
                  .HasMaxLength(200);

            entity.Property(e => e.Description)
                  .HasMaxLength(2000);

            entity.Property(e => e.Note)
                  .HasMaxLength(4000);

            entity.Property(e => e.State)
                  .HasConversion<string>()
                  .HasMaxLength(50);

            entity.HasMany(e => e.TimeEntries)
                  .WithOne(e => e.Task)
                  .HasForeignKey(e => e.TaskId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureTimeEntry(ModelBuilder modelBuilder)
    {
        ValueConverter<TimeSpan, long> timeSpanConverter = new(
            v => v.Ticks,
            v => TimeSpan.FromTicks(v));

        modelBuilder.Entity<TimeEntry>(entity =>
        {
            entity.ToTable("TimeEntries");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Code)
                  .IsRequired()
                  .HasMaxLength(50);

            entity.Property(e => e.Name)
                  .IsRequired()
                  .HasMaxLength(200);

            entity.Property(e => e.WorkDone)
                  .HasMaxLength(4000);

            entity.Property(e => e.Duration)
                  .HasConversion(timeSpanConverter);
        });
    }

    private static void ConfigureTodo(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Todo>(entity =>
        {
            entity.ToTable("Todos");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Code)
                  .IsRequired()
                  .HasMaxLength(50);

            entity.Property(e => e.Name)
                  .IsRequired()
                  .HasMaxLength(200);

            entity.Property(e => e.Description)
                  .HasMaxLength(2000);

            entity.Property(e => e.Note)
                  .HasMaxLength(4000);
        });
    }
}

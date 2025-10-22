using Microsoft.EntityFrameworkCore;

namespace AniVault.Database.Context;

public class AniVaultDbContext : DbContext
{
    private readonly IConfiguration _configuration;
    private readonly IHostEnvironment _hostEnvironment;

    public AniVaultDbContext(IConfiguration configuration, IHostEnvironment hostEnvironment)
    {
        _configuration = configuration;
        _hostEnvironment = hostEnvironment;
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseNpgsql(_configuration.GetConnectionString("AnimeVaultDB")).UseSnakeCaseNamingConvention();
        if (_hostEnvironment.IsDevelopment())
        {
            options.EnableSensitiveDataLogging();
        }
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AnimeConfiguration>()
            .HasIndex(ac => ac.AnimeName);
        
        modelBuilder.Entity<AnimeConfiguration>()
            .HasIndex(ac => ac.MyAnimeListId)
            .IsUnique(true);
        
        modelBuilder.Entity<TelegramMediaDocument>()
            .HasIndex(p => new {p.FileId, p.TelegramMessageId})
            .IsUnique(true);

        modelBuilder.Entity<TelegramChannel>()
            .HasIndex(p => p.ChatId)
            .IsUnique(true);
        
        modelBuilder.Entity<TelegramMessage>()
            .HasIndex(p => new {p.MessageId, p.TelegramChannelId})
            .IsUnique(true);
        modelBuilder.Entity<ApiUser>()
            .HasIndex(p => p.ApiKey)
            .IsUnique(true);

    }

    
    public virtual DbSet<TelegramChannel> TelegramChannels { get; set; }
    public virtual DbSet<TelegramMediaDocument> TelegramMediaDocuments { get; set; }
    public virtual DbSet<TelegramMessage> TelegramMessages { get; set; }
    public virtual DbSet<ScheduledTask> ScheduledTasks { get; set; }
    public virtual DbSet<AnimeConfiguration> AnimeEpisodesSettings { get; set; }
    public virtual DbSet<ApiUser> ApiUsers { get; set; }


    public void Migrate()
    {
        if (Database.GetPendingMigrations().Any())
        {
            Database.Migrate();
        }
    }
}
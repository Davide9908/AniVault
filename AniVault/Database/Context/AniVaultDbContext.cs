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
        modelBuilder.Entity<TelegramMediaDocument>()
            .HasIndex(p => new {p.FileId, p.TelegramMessageId})
            .IsUnique(true);

        modelBuilder.Entity<TelegramChannel>()
            .HasIndex(p => p.ChatId)
            .IsUnique(true);

        modelBuilder.Entity<SystemConfigurationParameter>()
            .HasIndex(p => p.ParameterName )
            .IsUnique(true);
        
        modelBuilder.Entity<TelegramMessage>()
            .HasIndex(p => new {p.MessageId, p.TelegramChannelId})
            .IsUnique(true);
        modelBuilder.Entity<ApiUser>()
            .HasIndex(p => p.ApiKey)
            .IsUnique(true);

        //modelBuilder.Entity<AnimeEpisodesSetting>(entity =>
        //{
        //    entity.HasKey(z => z.TelegramChannelId);
        //    entity.HasOne(p => p.TelegramChannel);
        //});
    }

    
    public virtual DbSet<TelegramChannel> TelegramChannels { get; set; }
    public virtual DbSet<SystemConfigurationParameter> SystemConfigurationParameters { get; set; }
    public virtual DbSet<TelegramMediaDocument> TelegramMediaDocuments { get; set; }
    public virtual DbSet<TelegramMessage> TelegramMessages { get; set; }
    public virtual DbSet<ScheduledTask> ScheduledTasks { get; set; }
    public virtual DbSet<AnimeEpisodesSetting> AnimeEpisodesSettings { get; set; }
    public virtual DbSet<ApiUser> ApiUsers { get; set; }


    public void Migrate()
    {
        if (Database.GetPendingMigrations().Any())
        {
            Database.Migrate();
        }
    }
}
using Microsoft.EntityFrameworkCore;

namespace AniVault.Database.Context;

public class AniVaultDbContext : DbContext
{
    private readonly IConfiguration _configuration;

    public AniVaultDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseNpgsql(_configuration.GetConnectionString("AnimeVaultDB")).UseSnakeCaseNamingConvention();
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TelegramMediaDocument>()
            .HasIndex(p => p.FileId)
            .IsUnique(true);

        modelBuilder.Entity<TelegramChannel>()
            .HasIndex(p => p.ChatId)
            .IsUnique(true);

        modelBuilder.Entity<SystemConfigurationParameter>()
            .HasIndex(p => p.ParameterName )
            .IsUnique(true);
        
        modelBuilder.Entity<TelegramMessage>()
            .HasIndex(p => p.MessageId)
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
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

        modelBuilder.Entity<AnimeConfiguration>()
            .HasIndex(ac => ac.FileNameTemplate);
        
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
        TelegramMediaDocumentOrder(modelBuilder);
    }

    private void TelegramMediaDocumentOrder(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TelegramMediaDocument>()
            .Property(md => md.TelegramMediaDocumentId)
            .HasColumnOrder(0);
        modelBuilder.Entity<TelegramMediaDocument>()
            .Property(md => md.FileId)
            .HasColumnOrder(1);
        modelBuilder.Entity<TelegramMediaDocument>()
            .Property(md => md.Filename)
            .HasColumnOrder(2);
        modelBuilder.Entity<TelegramMediaDocument>()
            .Property(md => md.DownloadStatus)
            .HasColumnOrder(3);
        modelBuilder.Entity<TelegramMediaDocument>()
            .Property(md => md.CreationDateTime)
            .HasColumnOrder(4);
        modelBuilder.Entity<TelegramMediaDocument>()
            .Property(md => md.LastUpdateDateTime)
            .HasColumnOrder(5);
        modelBuilder.Entity<TelegramMediaDocument>()
            .Property(md => md.Retries)
            .HasColumnOrder(6);
        modelBuilder.Entity<TelegramMediaDocument>()
            .Property(md => md.AnimeConfigurationId)
            .HasColumnOrder(7);
        modelBuilder.Entity<TelegramMediaDocument>()
            .Property(md => md.TelegramMessageId)
            .HasColumnOrder(8);
        modelBuilder.Entity<TelegramMediaDocument>()
            .Property(md => md.FilenameFromTelegram)
            .HasColumnOrder(9);
        modelBuilder.Entity<TelegramMediaDocument>()
            .Property(md => md.FilenameToUpdate)
            .HasColumnOrder(10);
        modelBuilder.Entity<TelegramMediaDocument>()
            .Property(md => md.MimeType)
            .HasColumnOrder(11);
        modelBuilder.Entity<TelegramMediaDocument>()
            .Property(md => md.Size)
            .HasColumnOrder(12);
        modelBuilder.Entity<TelegramMediaDocument>()
            .Property(md => md.DataTransmitted)
            .HasColumnOrder(13);
        modelBuilder.Entity<TelegramMediaDocument>()
            .Property(md => md.AccessHash)
            .HasColumnOrder(14);
        modelBuilder.Entity<TelegramMediaDocument>()
            .Property(md => md.FileReference)
            .HasColumnOrder(15);
    }

    
    public virtual DbSet<TelegramChannel> TelegramChannels { get; set; }
    public virtual DbSet<TelegramMediaDocument> TelegramMediaDocuments { get; set; }
    public virtual DbSet<TelegramMessage> TelegramMessages { get; set; }
    public virtual DbSet<ScheduledTask> ScheduledTasks { get; set; }
    public virtual DbSet<AnimeConfiguration> AnimeConfigurations { get; set; }
    public virtual DbSet<ApiUser> ApiUsers { get; set; }


    public void Migrate()
    {
        if (Database.GetPendingMigrations().Any())
        {
            Database.Migrate();
        }
    }
}
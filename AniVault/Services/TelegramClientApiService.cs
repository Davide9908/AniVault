using AniVault.Database;
using AniVault.Database.Context;
using AniVault.Database.Extensions;
using AniVault.Services.Extensions;
using Microsoft.EntityFrameworkCore;
using TL;

namespace AniVault.Services;

public class TelegramClientApiService
{
    private readonly ILogger<TelegramClientApiService> _log;
    private readonly TelegramClientService _tgClientService;
    private readonly AniVaultDbContext  _dbContext;

    private const int NumberOfMessageToCacheDuringLoading = 25;

    public TelegramClientApiService(ILogger<TelegramClientApiService> log, TelegramClientService tgClientService, AniVaultDbContext dbContext)
    {
        _log = log;
        _tgClientService = tgClientService;
        _dbContext = dbContext;
    }

    public async Task LoadMissingChannelsAndMessages(int userId, CancellationToken ct = default)
    {
        var apiUser = await _dbContext.ApiUsers.GetById(userId).FirstAsync(ct);
        _log.Info("Api user {apiUser} requested to load all channel with their messages", apiUser.ToString());

        if (_dbContext.TelegramChannels.Any())
        {
            throw new InvalidOperationException("Cannot load channels: there already exists channel in the database");
        }

        var tgChannels = _tgClientService.GetCachedChannels();
        List<TelegramChannel> channels = new List<TelegramChannel>(tgChannels.Count);
        foreach (var tgChannel in tgChannels)
        {
            var newChannel = new TelegramChannel(tgChannel.ID, tgChannel.access_hash, tgChannel.Title)
            {
                Status = tgChannel.flags.HasFlag(Channel.Flags.left) ? ChannelStatus.Deleted : ChannelStatus.Active
            };
            channels.Add(newChannel);
            if (newChannel.Status == ChannelStatus.Active)
            {
                continue;
            }
            
            var tgMessages = (await _tgClientService.GetLastChannelMessages(NumberOfMessageToCacheDuringLoading, tgChannel)).OfType<Message>().ToList();
            foreach (Message tgMessage in tgMessages)
            {
                TelegramMessage telegramMessage = new()
                {
                    ReceivedDatetime = tgMessage.Date,
                    UpdateDatetime = tgMessage.edit_date.Ticks != 0 ? tgMessage.edit_date : null,
                    MessageText = tgMessage.message,
                    MessageStatus = MessageStatus.Active
                };
                newChannel.TelegramMessages.Add(telegramMessage);
                
                if (tgMessage.media is null)
                {
                    continue;
                }
                
                var mediaDocument = (tgMessage.media as MessageMediaDocument)?.document as Document;
                if (mediaDocument is null)
                {
                    continue;
                }
                
                telegramMessage.MediaDocument = new(mediaDocument.ID, mediaDocument.access_hash, mediaDocument.file_reference, mediaDocument.Filename, mediaDocument.Filename, mediaDocument.size, mediaDocument.mime_type)
                {
                    DownloadStatus = DownloadStatus.Ignored
                };
                await Task.Delay(Random.Shared.Next(1000, 15000), ct);
                ct.ThrowIfCancellationRequested();
            }
        }
        
        _dbContext.TelegramChannels.AddRange(channels);
        await _dbContext.SaveChangesAsync(ct);
    }
}
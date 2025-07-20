using System.Text.RegularExpressions;
using AniVault.Database;
using AniVault.Database.Context;
using AniVault.Services.Classes;
using AniVault.Services.Extensions;
using Microsoft.EntityFrameworkCore;
using TL;

namespace AniVault.Services;

public partial class TelegramClientService
{
    private async Task Client_OnUpdate(Update update)
    {
        using var scope = _serviceProvider.CreateScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<AniVaultDbContext>();
        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        switch (update)
        {
            case UpdateNewChannelMessage ncm:
                await HandleNewChannelMessageUpdate(ncm, dbContext);
                break;
            case UpdateEditChannelMessage uecm: //tutti gli aggiornamenti di update di messaggi
                HandleEditChannelMessageUpdate(uecm, dbContext);
                break;
            case UpdateDeleteChannelMessages udcm: 
                HandleDeleteChannelMessagesUpdate(udcm, dbContext);
                break;
            case UpdateChannel channelUpdate:
                HandleChannelUpdate(channelUpdate, dbContext);
                break;
            default:
                _log.Debug($"Unmanaged update received: {update.GetType().Name}");
                break;
            //case UpdateUserTyping uut: Console.WriteLine($"{User(uut.user_id)} is {uut.action}"); break;
            //case UpdateChatUserTyping ucut: Console.WriteLine($"{Peer(ucut.from_id)} is {ucut.action} in {Chat(ucut.chat_id)}"); break;
            //case UpdateChannelUserTyping ucut2: Console.WriteLine($"{Peer(ucut2.from_id)} is {ucut2.action} in {Chat(ucut2.channel_id)}"); break;
            //case UpdateChatParticipants { participants: ChatParticipants cp }: Console.WriteLine($"{cp.participants.Length} participants in {Chat(cp.chat_id)}"); break;
            //case UpdateUserStatus uus: Console.WriteLine($"{User(uus.user_id)} is now {uus.status.GetType().Name[10..]}"); break;
            //case UpdateUserName uun: Console.WriteLine($"{User(uun.user_id)} has changed profile name: {uun.first_name} {uun.last_name}"); break;
            //case UpdateUser uu: Console.WriteLine($"{User(uu.user_id)} has changed infos/photo"); break;
            
        }

        await transaction.CommitAsync();
    }

    private async Task Client_OnOther(IObject arg)
    {
        switch (arg)
        {
            case ReactorError err:
                // typically: network connection was totally lost
                _log.Error(err.Exception, "Fatal reactor error");
                while (true)
                {
                    _log.Error("Disposing the client and trying to reconnect in 5 seconds...");
                    await DisconnectAndClearAsync();
                    await Task.Delay(5000);
                    try
                    {
                        User? user = await Connect(true);
                        if (user is null)
                        {
                            throw new InvalidOperationException(
                                "Connection completed with no error, but returned user is null");
                        }

                        _log.Info("Connected with user {userId} - {username}", user.ID, user.MainUsername);
                        break;
                    }
                    catch (Exception ex)
                    {
                        _log.Error(ex, "Connection still failing");
                    }
                }

                break;
            case Pong:
                break;
            default:
                _log.Warning($"Client_OnOther: Other - {arg.GetType().Name}");
                break;
        }
    }

    private void HandleChannelUpdate(UpdateChannel channelUpdate, AniVaultDbContext dbContext)
    {
        ChatBase chatBase;
        if (!_updateManager.Chats.TryGetValue(channelUpdate.channel_id, out chatBase!))
        {
            _log.Warning(
                "Received a {updateType} update, but the channel id was not found in the update manager",
                nameof(UpdateChannel));
            return;
        }

        if (chatBase is not Channel channel)
        {
            throw new InvalidOperationException($"Received {nameof(UpdateChannel)} update, but the chatbase extracted is not a Channel");
        }

        var dbChannel = dbContext.TelegramChannels.FirstOrDefault(c => c.ChatId == channel.ID);
        if (dbChannel is null)
        {
            TelegramChannel newChannel = new(channel.ID, channel.access_hash, channel.Title);
            if (channel.flags.HasFlag(Channel.Flags.left))
            {
                newChannel.Status = ChannelStatus.Deleted;
            }
            dbContext.TelegramChannels.Add(newChannel);
            dbContext.SaveChanges();
            return;
        }

        if (channel.flags.HasFlag(Channel.Flags.left) && dbChannel.Status != ChannelStatus.Deleted)
        {
            dbChannel.Status = ChannelStatus.Deleted;
            dbChannel.AutoDownloadEnabled = false;
        }

        if (!channel.flags.HasFlag(Channel.Flags.left) && dbChannel.Status == ChannelStatus.Deleted)
        {
            dbChannel.Status = ChannelStatus.Active;
            dbChannel.AccessHash = channel.access_hash; //Update the access hash just in case
        }

        if (channel.Title != dbChannel.ChannelName)
        {
            dbChannel.ChannelName = channel.Title;
        }

        dbContext.SaveChanges();

    }

    private async Task HandleNewChannelMessageUpdate(UpdateNewChannelMessage newChannelMessage, AniVaultDbContext dbContext)
    {
        
        ChatBase chatBase;
        if (!_updateManager.Chats.TryGetValue(newChannelMessage.message.Peer.ID, out chatBase!))
        {
            _log.Warning(
                "Received a {updateType} update, but the channel id was not found in the update manager",
                nameof(UpdateNewChannelMessage));
            return;
        }
        
        if (chatBase is not Channel channel)
        {
            throw new InvalidOperationException(
                $"Received {nameof(UpdateNewChannelMessage)} update, but the chatbase extracted is not a Channel");
        }
        if (newChannelMessage.message is not Message message)
        {
            throw new InvalidOperationException(
                $"Received {nameof(UpdateNewChannelMessage)} update, but the messageBase from the update is not a Message");
        }
        
        var dbChannel = dbContext.TelegramChannels
            .Include(tc => tc.AnimeEpisodesSetting)
            .FirstOrDefault(c => c.ChatId == channel.ID);
        
        _log.Info("Update has been received from channel {channelInfo} with message {message}", dbChannel, message.message);

        if (dbChannel is null)
        {
            _log.Error("Received a {updateType} update, but the channel {id} - {channelTitle} was not found inside the database", nameof(UpdateNewChannelMessage), channel.id , channel.Title);
            return;
        }
        
        //If channel was sent with min flag, I load the access has in order to do the readHistory later
        if (channel.flags.HasFlag(Channel.Flags.min))
        {
            channel.access_hash = dbChannel.AccessHash;
        }

        TelegramMessage newMessage = new TelegramMessage()
        {
            MessageId = message.ID,
            MessageText = message.message,
            TelegramChannel = dbChannel
        };
        
        dbContext.TelegramMessages.Add(newMessage);
        if (message.media is MessageMediaDocument { document: Document document }) //It's the same as message.media is MessageMediaDocument var1 && var1.document is Document var 2
        {
            string filename = GetFilenameByMessage(dbChannel.AnimeEpisodesSetting, message.message, document.Filename);
            TelegramMediaDocument newMediaDocument = new(document.ID, document.access_hash, document.file_reference,
                newMessage, filename, document.Filename, document.size, document.mime_type);
            dbContext.TelegramMediaDocuments.Add(newMediaDocument);
        }
        await ReadChannelHistory(channel, message.ID); //mark message as read
        
        await dbContext.SaveChangesAsync();
    }

    private string GetFilenameByMessage(AnimeEpisodesSetting? episodesSetting, string messageText, string filenameFromTelegram)
    {
        Match match = RegexUtils.EpRegex().Match(messageText);
        if (!match.Success)
        {
            _log.Warning("Ep number could not be extrapolated from message: {message}", messageText);
            return filenameFromTelegram;
        }
        if (episodesSetting is null || string.IsNullOrWhiteSpace(episodesSetting.FileNameTemplate))
        {
            return filenameFromTelegram;
        }
        
        string epNumber = match.Value.ToLowerInvariant().Replace("#ep", "");
        if (epNumber.Length == 1) //if the ep number is single digit (0-9), I add the 0 in front of it (01, 02, 03 etc...)
        {
            epNumber = epNumber.PadLeft(2, '0');
        }
        
        string extension = "." + RegexUtils.FileExtensionRegex().Match(filenameFromTelegram).Value;
        
        if (episodesSetting.CourEpisodeNumberGap.HasValue && episodesSetting.UseGapForEpNum)
        {
            epNumber = (int.Parse(epNumber) + episodesSetting.CourEpisodeNumberGap.Value).ToString();
        }
        return episodesSetting.FileNameTemplate + epNumber + extension;
    }

    private void HandleEditChannelMessageUpdate(UpdateEditChannelMessage update, AniVaultDbContext dbContext)
    {
        if (update.message is not Message message)
        {
            throw new InvalidOperationException(
                $"Received {nameof(UpdateNewChannelMessage)} update, but the messageBase from the update is not a Message");
        }
        var dbMessage = dbContext.TelegramMessages
            .Include(tm=>tm.TelegramChannel)
                .ThenInclude(c => c.AnimeEpisodesSetting)
            .Include(tm => tm.MediaDocument)
            .FirstOrDefault(tm => tm.MessageId == message.ID && tm.TelegramChannel.ChatId == message.Peer.ID);
        if (dbMessage is null)
        {
            _log.Warning("Received channel message update for message {id} - {messageText}, but the message doesn't exist in the database",  message.ID, message.message);
            return;
        }

        if (dbMessage.MessageText == message.message)
        {
            return;
        }

        dbMessage.UpdateDatetime = message.edit_date;
        
        dbMessage.MessageText = message.message;
        if (dbMessage.MediaDocument is not null 
            && dbMessage.MediaDocument.Filename == dbMessage.MediaDocument.FilenameFromTelegram 
            && dbMessage.TelegramChannel.AnimeEpisodesSetting is not null)
        {
            string newFilename = GetFilenameByMessage(dbMessage.TelegramChannel.AnimeEpisodesSetting, message.message, dbMessage.MediaDocument.FilenameFromTelegram);
            if (newFilename != dbMessage.MediaDocument.FilenameFromTelegram)
            {
                //If the media is not being downloaded, I just update his filename
                if (dbMessage.MediaDocument.DownloadStatus == DownloadStatus.Downloading)
                {
                    dbMessage.MediaDocument.FilenameToUpdate = newFilename;
                }
                else
                {
                    dbMessage.MediaDocument.Filename = newFilename;
                }
            }
        }

        dbContext.SaveChanges();

    }

    private void HandleDeleteChannelMessagesUpdate(UpdateDeleteChannelMessages update, AniVaultDbContext dbContext)
    {
        var channel = dbContext.TelegramChannels
            .Include(tc => tc.TelegramMessages)
            .FirstOrDefault(tc => tc.ChatId == update.channel_id);
        if (channel is null)
        {
            _log.Warning("Received a {updateType} update, but the channel id {id} was not found in the database", nameof(UpdateDeleteChannelMessages), update.channel_id);
            return;
        }

        foreach (int messageId in update.messages)
        {
            var message = channel.TelegramMessages.FirstOrDefault(tm=>tm.MessageId == messageId);
            if (message is null)
            {
                _log.Warning("Received a {updateType} update, but the message id {id} was not found in the database", nameof(UpdateDeleteChannelMessages), messageId);
                continue;
            }
            message.Delete();
        }
        dbContext.SaveChanges();
    }
}
using AniVault.Services.Classes.Exceptions;
using AniVault.Services.Extensions;
using TL;
using WTelegram;

namespace AniVault.Services;

public partial class TelegramClientService
{
    public ChatBase? GetCachedChatById(long chatId)
    {
        _updateManager.Chats.TryGetValue(chatId, out var chat);
        return chat;
        //return _allChats?.FirstOrDefault(c => c.ID == chatId);
    }
    public List<ChatBase> GetCachedChats() => _updateManager.Chats.Values.ToList();
    
    public List<Channel> GetCachedChannels() => GetCachedChats().OfType<Channel>().ToList();

    public async Task<MessageBase[]> GetChannelHistory(InputPeer channelPeer)
    {
        return (await _tgClient.Messages_GetHistory(channelPeer)).Messages;
    }


    /// <summary>
    /// Read history of channel
    /// </summary>
    /// <param name="channel">The channel to set as read</param>
    /// <param name="messageId">Message's id to set as read</param>
    private async Task ReadChannelHistory(Channel channel, int messageId)
    {
        int time = Random.Shared.Next(500, 5000);
        await Task.Delay(time);

        try
        {
            await _tgClient.Channels_ReadHistory(new InputChannel(channel.id, channel.access_hash), messageId);
        }
        catch (RpcException ex)
        {
            _log.Error("An error occured on ReadChannelHistory", ex);
        }
    }

    /// <summary>
    /// Download file from Telegram and automatically catch any exception.
    /// </summary>
    /// <param name="document">The file to download from Telegram</param>
    /// <param name="outputStream">Output file stream where to save the data. If exception is thown dispose is handled automatically</param>
    /// <param name="progress">Process callback method</param>
    /// <exception cref="Exception"></exception>
    /// <returns>true if the client is connected and the download succeeded, otherwise false</returns>
    public async Task<bool> TryDownloadFileAsync(Document document, FileStream outputStream, Client.ProgressCallback? progress = null)
    {
        try
        {
            if (_tgClient is null || _tgClient.Disconnected)
            {
                throw new TelegramClientDisconnectedException();
            }
            
            await DownloadFileAsync(document, outputStream, progress);
            return true;
        }
        catch (Exception ex)
        {
            _log.Error($"An error occured while downloading file {document.Filename} to {outputStream.Name}", ex);
            return false;
        }
        finally
        {
            outputStream.Flush();
            await outputStream.DisposeAsync();
        }
    }

    /// <summary>
    /// Download file from Telegram
    /// </summary>
    /// <param name="document">The file to download from Telegram</param>
    /// <param name="outputStream">Output file stream where to save the data</param>
    /// <param name="progress">Process callback method</param>
    /// <exception cref="Exception"/>
    /// <returns>false if client is not connected otherwise true</returns>
    public async Task DownloadFileAsync(Document document, FileStream outputStream, Client.ProgressCallback? progress = null, bool disposeOnError = true)
    {
        try
        {
            if (_tgClient is null || _tgClient.Disconnected)
            {
                throw new TelegramClientDisconnectedException();
            }

            await _tgClient.DownloadFileAsync(document, outputStream, null, progress);
        }
        finally
        {
            if (disposeOnError)
            {
                outputStream.Flush();
                await outputStream.DisposeAsync();
            }
        }
        
    }

    public Task<Messages_MessagesBase?> GetChannelMessagesByIds(long channelId, long channelAccessHash, List<int> messageIds)
    {
        if (_tgClient is null || _tgClient.Disconnected)
        {
            throw new TelegramClientDisconnectedException();
        }

        InputPeerChannel inputPeerChannel = new InputPeerChannel(channelId, channelAccessHash);
        
        List<InputMessage> inputMessageIDs = messageIds.Select(id =>
            new InputMessageID()
            {
                id = id
            } as InputMessage
        ).ToList();

        return _tgClient.GetMessages(inputPeerChannel, inputMessageIDs.ToArray());
    }

    public async Task<MessageBase[]> GetLastChannelMessages(int count, Channel tgChannel)
    {
        if (count == 0)
        {
            return [];
        }

        var history = await _tgClient.Messages_GetHistory(tgChannel.ToInputPeer(), offset_date: DateTime.Now, limit:count);
        return history.Messages;

    }
    
    public async Task<MessageBase[]> GetChannelMessagesFromId(int messageIdOffset, InputPeerChannel tgChannel)
    {
        var history = await _tgClient.Messages_GetHistory(tgChannel, min_id: messageIdOffset);
        return history.Messages;
    }
}
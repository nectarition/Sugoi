using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sugoi.Functions.Configurations;
using Sugoi.Functions.Models.Discord;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Sugoi.Functions.Services.Common;

public interface IDiscordService
{
    Task<GuildChannel[]> GetChannelsAsync();
    Task<Message[]> GetMessagesAsync(string channelId);
    Task<Message[]> GetMessagesAsync(string channelId, string sinceMessageId);
}

public class DiscordService : IDiscordService
{
    private ILogger Logger { get; }
    private SugoiConfiguration SugoiConfiguration { get; }
    private HttpClient HttpClient { get; }

    private const string GetGuildChannelsEndpoint = "https://discord.com/api/v10/guilds/{0}/channels";
    private const string GetChannelMessagesEndpoint = "https://discord.com/api/v10/channels/{0}/messages";

    public DiscordService(ILoggerFactory loggerFactory,
        IOptions<SugoiConfiguration> sugoiConfigurationOptions)
    {
        Logger = loggerFactory.CreateLogger<DiscordService>();
        SugoiConfiguration = sugoiConfigurationOptions.Value;
        HttpClient = new HttpClient();
    }

    public async Task<GuildChannel[]> GetChannelsAsync()
    {
        var channels = await GetCoreAsync<GuildChannel[]>(
            string.Format(GetGuildChannelsEndpoint, SugoiConfiguration.Aggregates.GuildId));
        if (channels == null)
        {
            throw new ArgumentNullException(nameof(channels));
        }

        return channels;
    }

    public async Task<Message[]> GetMessagesAsync(string channelId)
    {
        var messages = await GetCoreAsync<Message[]>(string.Format(GetChannelMessagesEndpoint, channelId));
        if (messages == null)
        {
            throw new ArgumentNullException(nameof(messages));
        }

        return messages;
    }

    public async Task<Message[]> GetMessagesAsync(string channelId, string sinceMessageId)
    {
        var messages = await GetCoreAsync<Message[]>($"{string.Format(GetChannelMessagesEndpoint, channelId)}?after={sinceMessageId}");
        if (messages == null)
        {
            throw new ArgumentNullException(nameof(messages));
        }

        return messages;
    }

    async Task<T?> GetCoreAsync<T>(string endpointUrl)
    {
        var req = new HttpRequestMessage(HttpMethod.Get, endpointUrl);
        req.Headers.Add("Authorization", $"Bot {SugoiConfiguration.Secrets.BotToken}");

        Logger.LogInformation($"Request: GET {endpointUrl}");
        var res = await HttpClient.SendAsync(req);
        var data = await res.Content.ReadFromJsonAsync<T>();

        return data;
    }

    async Task PostCoreAsync(string endpointUrl, Dictionary<string, object> param)
    {
        var json = JsonSerializer.Serialize(param);
        var content = new StringContent(json, Encoding.UTF8, @"application/json");

        var req = new HttpRequestMessage(HttpMethod.Post, endpointUrl)
        {
            Content = content
        };
        req.Headers.Add("Authorization", $"Bot {SugoiConfiguration.Secrets.BotToken}");

        var res = await HttpClient.SendAsync(req);
        var data = await res.Content.ReadAsStringAsync();

        Console.WriteLine(data);
    }
}

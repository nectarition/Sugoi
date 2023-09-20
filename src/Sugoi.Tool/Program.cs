﻿using System.Text;
using System.Text.Json;
using static System.Net.WebRequestMethods;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Sugoi.Tool;

internal class Program
{
    private static HttpClient HttpClient { get; }

    const string CommandAddEndpointFormat = "https://discord.com/api/v8/applications/{0}/guilds/{1}/commands";
    const string CommandForGuildUpdateEndpointFormat = "https://discord.com/api/v8/applications/{0}/guilds/{1}/commands/{2}";

    static Program()
    {
        HttpClient = new HttpClient();
    }

    static void Main(string[] args)
    {
        Console.Write("AppId: ");
        var appId = Console.ReadLine();
        if (string.IsNullOrEmpty(appId) )
        {
            throw new ArgumentNullException(nameof(appId));
        }

        Console.Write("GuildId: ");
        var guildId = Console.ReadLine();
        if (string.IsNullOrEmpty(guildId))
        {
            throw new ArgumentNullException(nameof(guildId));
        }

        Console.Write("CommandId: ");
        var commandId = Console.ReadLine();
        if (string.IsNullOrEmpty(commandId))
        {
            throw new ArgumentNullException(nameof(commandId));
        }

        Console.Write("BotToken: ");
        var botToken = Console.ReadLine();
        if (string.IsNullOrEmpty(botToken))
        {
            throw new ArgumentNullException(nameof(botToken));
        }

        Console.WriteLine("\n以下の内容で登録します。よろしいですか？ (yes / no)\n");

        Console.WriteLine("----- 接続情報");
        Console.WriteLine($"AppId: {appId}");
        Console.WriteLine($"GuildId: {guildId}");
        Console.WriteLine($"CommandId: {commandId}");
        Console.WriteLine($"BotToken: {botToken}");

        var confirm = Console.ReadLine();
        if (string.IsNullOrEmpty(confirm) || !confirm.Contains("yes"))
        {
            return;
        }

        var task = UpdateAsync(appId, guildId, commandId, botToken);
        task.Wait();

        Console.WriteLine("登録しました。");
    }

    static async Task RegisterAsync(string appId, string guildId, string botToken)
    {
        var endpointUrl = string.Format(CommandAddEndpointFormat, appId, guildId);

        var registerParam = new Dictionary<string, object>()
        {
            ["name"] = "sugoi",
            ["description"] = "すごい",
            ["options"] = new Dictionary<string, object>[]
            {
                new()
                {
                    ["name"] = "ping",
                    ["description"] = "pong",
                    ["type"] = 1
                }
            }
        };

        await PostCoreAsync(endpointUrl, registerParam, botToken);
    }

    static async Task UpdateAsync(string appId, string guildId, string commandId, string botToken)
    {
        var endpointUrl = string.Format(CommandForGuildUpdateEndpointFormat, appId, guildId, commandId);

        var updateParam = new Dictionary<string, object>()
        {
            ["options"] = new Dictionary<string, object>[]
            {
                new()
                {
                    ["name"] = "ping",
                    ["description"] = "pong",
                    ["type"] = 1
                },
                new()
                {
                    ["name"] = "sugoi",
                    ["description"] = "すごい",
                    ["type"] = 1
                },
                new()
                {
                    ["name"] = "jomei",
                    ["description"] = "除名",
                    ["type"] = 1
                }
            }
        };

        await PatchCoreAsync(endpointUrl, updateParam, botToken);
    }

    static async Task PostCoreAsync(string endpointUrl, Dictionary<string, object> param, string botToken)
    {
        var json = JsonSerializer.Serialize(param);
        var content = new StringContent(json, Encoding.UTF8, @"application/json");

        var req = new HttpRequestMessage(HttpMethod.Post, endpointUrl)
        {
            Content = content
        };
        req.Headers.Add("Authorization", $"Bot {botToken}");

        var res = await HttpClient.SendAsync(req);
        var data = await res.Content.ReadAsStringAsync();

        Console.WriteLine(data);
    }
    static async Task PatchCoreAsync(string endpointUrl, Dictionary<string, object> param, string botToken)
    {
        var json = JsonSerializer.Serialize(param);
        var content = new StringContent(json, Encoding.UTF8, @"application/json");

        var req = new HttpRequestMessage(HttpMethod.Patch, endpointUrl)
        {
            Content = content
        };
        req.Headers.Add("Authorization", $"Bot {botToken}");

        var res = await HttpClient.SendAsync(req);
        var data = await res.Content.ReadAsStringAsync();

        Console.WriteLine(data);
    }
}

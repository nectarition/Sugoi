using System.Text;
using System.Text.Json;
using static System.Net.WebRequestMethods;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Sugoi.Tool;

internal class Program
{
    private static HttpClient HttpClient { get; }

    const string CommandAddEndpointFormat = "https://discord.com/api/v8/applications/{0}/guilds/{1}/commands";

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
        Console.WriteLine($"BotToken: {botToken}");

        var confirm = Console.ReadLine();
        if (string.IsNullOrEmpty(confirm) || !confirm.Contains("yes"))
        {
            return;
        }

        var task = RegisterAsync(appId, guildId, botToken);
        task.Wait();

        Console.WriteLine("登録しました。");
    }

    static async Task RegisterAsync(string appId, string guildId, string botToken)
    {
        var endpointUrl = string.Format(CommandAddEndpointFormat, appId, guildId);

        var registerParams = new Dictionary<string, object>()
        {
            ["name"] = "sugoi",
            ["description"] = "すごい",
            ["options"] = new Dictionary<string, object>[]
            {
                new() {
                    ["name"] = "ping",
                    ["description"] = "pong",
                    ["type"] = 1
                }
            }
        };

        var json = JsonSerializer.Serialize(registerParams);
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
}

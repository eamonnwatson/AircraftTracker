using AircraftTracker.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AircraftTracker.Pushover;

internal enum Priority
{
    Lowest = -2,
    Low = -1,
    Normal = 0,
    High = 1,
    Emergency = 2
}
internal class PushoverClient : IPushoverClient
{

    private const string BASE_API_URL = "https://api.pushover.net/1/messages.json";
    private readonly HttpClient Client = new();
    public string AppKey { get; set; }
    public string? DefaultUserKey { get; set; }
    public PushoverClient(string appKey)
    {
        AppKey = appKey;
    }

    public PushoverClient(string appKey, string? defaultUserKey) : this(appKey)
    {
        DefaultUserKey = defaultUserKey;
    }

    public PushResponse Push(string title, string message)
    {
        var task = PushAsync(title, message, DefaultUserKey);
        task.Wait();
        return task.Result;
    }

    public async Task<PushResponse> PushAsync(string title, string message, string? userKey = null, Priority priority = Priority.Normal)
    {
        if (userKey is null && DefaultUserKey is null)
            throw new ArgumentNullException(nameof(userKey), "User Key was not supplied");
        
        using var content = new MultipartFormDataContent()
        {
            { new StringContent(AppKey), "token" },
            { new StringContent(userKey ?? DefaultUserKey!), "user" },
            { new StringContent(message), "message" },
            { new StringContent(priority.ToString()), "priority" },
            { new StringContent(title), "title" },
        };

        try
        {
            var result = await Client.PostAsync(BASE_API_URL, content);
            return await ReadStreamAsync(await result.Content.ReadAsStreamAsync());
        }
        catch (WebException webEx)
        {
            if (webEx.Response is not null)
                return await ReadStreamAsync(webEx.Response.GetResponseStream());

            return new PushResponse() { Request = "ERROR", Errors = new List<string>() { webEx.Message } };
        }

    }

    private static async Task<PushResponse> ReadStreamAsync(Stream stream)
    {
        using var sr = new StreamReader(stream);
        var retVal = JsonConvert.DeserializeObject<PushResponse>(await sr.ReadToEndAsync());

        if (retVal is null)
            return new PushResponse() { Request = "ERROR", Errors = new List<string>() { "Response was null" } };

        return retVal;
    }

}

using AircraftTracker.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace AircraftTracker.Pushover;

internal class PushoverClient(HttpClient client, IOptions<PushoverOptions> options) : IPushoverClient
{
    private readonly HttpClient _client = client;
    private readonly PushoverOptions _options = options.Value;

    public async Task<PushResponse> PushAsync(string title, string message)
    {        
        using var content = new MultipartFormDataContent()
        {
            { new StringContent(_options.APIToken), "token" },
            { new StringContent(_options.UserKey), "user" },
            { new StringContent(message), "message" },
            { new StringContent(title), "title" },
        };

        using var result = await _client.PostAsync(_client.BaseAddress, content);
        return await ReadStreamAsync(await result.Content.ReadAsStreamAsync());

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

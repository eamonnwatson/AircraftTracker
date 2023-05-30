using AircraftTracker.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AircraftTracker;
internal class WritableOptions<T> : IWritableOptions<T> where T : class, new()
{
    private readonly IHostEnvironment environment;
    private readonly IOptionsMonitor<T> options;
    private readonly IConfigurationRoot configuration;
    private readonly string section;
    private readonly string file;

    public WritableOptions(IHostEnvironment environment, IOptionsMonitor<T> options, IConfigurationRoot configuration, string section, string file)
    {
        this.environment = environment;
        this.options = options;
        this.configuration = configuration;
        this.section = section;
        this.file = file;
    }

    public T Value => options.CurrentValue;
    public T Get(string name) => options.Get(name);
    public void Update(Action<T> applyChanges)
    {
        var fileprovider = environment.ContentRootFileProvider;
        var fileinfo = fileprovider.GetFileInfo(file);
        var physicalPath = fileinfo.PhysicalPath!;

        var jObject = JsonConvert.DeserializeObject<JObject>(File.ReadAllText(physicalPath));
        var sectionObject = jObject.TryGetValue(section, out JToken sec) ? JsonConvert.DeserializeObject<T>(sec.ToString()) : (Value ?? new T());

        applyChanges(sectionObject);

        jObject[section] = JObject.Parse(JsonConvert.SerializeObject(sectionObject));
        File.WriteAllText(physicalPath, JsonConvert.SerializeObject(jObject, Formatting.Indented));
        configuration.Reload();
    }
}

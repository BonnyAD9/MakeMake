using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace MakeMake;

[JsonObject(MemberSerialization.OptIn)]
internal class Config
{
    public static Config Current { get; set; }
#if DEBUG
    public static string DirPath { get; } = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MakeMake", "debug");
#else
    public static string DirPath { get; } = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MakeMake");
#endif
    public static string FilePath { get; } = Path.Join(DirPath, "Config.json");

    static Config()
    {
        if (!Directory.Exists(DirPath))
            Directory.CreateDirectory(DirPath);
        if (!File.Exists(FilePath))
            File.Create(FilePath);

        Config? c = JsonConvert.DeserializeObject<Config>(File.ReadAllText(FilePath));
        if (c is null)
        {
            //throw new Exception("Failed to load config");
            Current = new();
            //JsonConvert.SerializeObject(Current, Formatting.Indented);
            return;
        }
        Current = c;
    }

    public static void Save() => File.WriteAllText(FilePath, JsonConvert.SerializeObject(Current, Formatting.Indented));

    /*[JsonProperty("$schema")]
    public string Schema { get; } = Path.Join(DirPath, "ConfigSchema.json");*/
    [JsonProperty(Required = Required.DisallowNull)]
    public string Compiler { get; set; } = "clang";
    [JsonProperty(Required = Required.DisallowNull)]
    public string DebugFlags { get; set; } = "-Wall -g -std=c17";
    [JsonProperty(Required = Required.DisallowNull)]
    public string ReleaseFlags { get; set; } = "-std=c17 -DNDEBUG -O3";
    [JsonProperty(Required = Required.DisallowNull)]
    public string MainName { get; set; } = "main";
    [JsonProperty]
    public string? OutName { get; set; } = null;
    [JsonProperty(Required = Required.DisallowNull)]
    public string Extension { get; set; } = ".exe";
    [JsonProperty(Required = Required.DisallowNull)]
    public List<Template> Templates { get; } = new();
}

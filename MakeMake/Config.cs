using Bny.Console;
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
            Term.Form(Term.brightRed, "error: ", Term.reset, $"invalid json data in {FilePath}");
            c = new();
        }

        Current = c;
        SetDefaults(Current);
    }

    private static void SetDefaults(Config c)
    {
        c.Variables.SetIfEmpty("MainName", "main");
        c.Variables.SetIfEmpty("Extension", ".exe");
        c.Variables.SetIfEmpty(" ", "");
    }

    public static void Save() => File.WriteAllText(FilePath, JsonConvert.SerializeObject(Current, Formatting.Indented));

    [JsonProperty(Required = Required.DisallowNull)]
    public Dictionary<string, string> Variables { get; set; } = new();

    [JsonProperty(Required = Required.DisallowNull)]
    public List<Template> Templates { get; } = new();

    public Dictionary<string, string> RVars { get; private set; } = new();
    public Dictionary<string, string>? TVars { get; set; }
}

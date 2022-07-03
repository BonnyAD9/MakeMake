using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace MakeMake;

[DataContract]
internal class Config
{
    public static Config Current { get; set; }
    public static string DirPath { get; } = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MakeMake");
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
            Current = new();
            File.WriteAllText(FilePath, JsonConvert.SerializeObject(Current, Formatting.Indented));
            return;
        }
        Current = c;
    }

    [DataMember]
    public string Compiler { get; set; } = "clang";
    [DataMember]
    public string DebugFlags { get; set; } = "-Wall -g -std=c17";
    [DataMember]
    public string ReleaseFlags { get; set; } = "-std=c17 -DNDEBUG -O3";
    [DataMember]
    public string? OutName { get; set; } = null;
    [DataMember]
    public string Extension { get; set; } = ".exe";
}

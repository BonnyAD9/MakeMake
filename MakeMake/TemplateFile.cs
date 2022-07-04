using Newtonsoft.Json;

namespace MakeMake;

[JsonObject(MemberSerialization.OptIn)]
internal class TemplateFile
{
    [JsonProperty(Required = Required.Always)]
    public string Name { get; set; }
    //public bool Parse { get; set; } = true;
    [JsonProperty(Required = Required.DisallowNull)]
    public string[] Contents { get; private set; } = Array.Empty<string>();

    [JsonConstructor]
    public TemplateFile(string name)
    {
        Name = name;
    }

    public TemplateFile(FileInfo file)
    {
        Name = file.Name;
        Contents = File.ReadAllLines(file.FullName);
    }

    public void Create(string directory)
    {
        File.WriteAllLines(Path.Join(directory, Name), Contents);
    }
}

using Newtonsoft.Json;

namespace MakeMake;

[JsonObject(MemberSerialization.OptIn)]
internal class TemplateFolder
{
    [JsonProperty(Required = Required.Always)]
    public string Name { get; set; }
    [JsonProperty(Required = Required.DisallowNull)]
    public TemplateFile[] Files { get; private set; } = Array.Empty<TemplateFile>();
    [JsonProperty(Required = Required.DisallowNull)]
    public TemplateFolder[] Folders { get; private set; } = Array.Empty<TemplateFolder>();

    public TemplateFolder(string name)
    {
        Name = name;
    }

    public void Create(string directory)
    {
        string path = Path.Join(directory, Name);
        Directory.CreateDirectory(path);
        foreach (var f in Files)
            f.Create(path);
        foreach (var d in Folders)
            d.Create(path);
    }
}

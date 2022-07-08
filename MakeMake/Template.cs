using Newtonsoft.Json;

namespace MakeMake;

[JsonObject(MemberSerialization.OptIn)]
internal class Template
{
    [JsonProperty(Required = Required.Always)]
    public string Name { get; set; }

    [JsonProperty(Required = Required.DisallowNull)]
    public string Description { get; set; } = "";

    [JsonProperty(Required = Required.DisallowNull)]
    public Dictionary<string, string> Variables { get; set; } = new();

    [JsonProperty(Required = Required.DisallowNull)]
    public TemplateFile[] Files { get; private set; } = Array.Empty<TemplateFile>();

    [JsonProperty(Required = Required.DisallowNull)]
    public TemplateFolder[] Folders { get; private set; } = Array.Empty<TemplateFolder>();

    [JsonConstructor]
    public Template(string name)
    {
        Name = name;
    }

    public void Create(bool raw = false)
    {
        Config.Current.TVars = Variables;

        if (raw)
            File.WriteAllText("'.json", JsonConvert.SerializeObject(Variables, Formatting.Indented));

        foreach (var f in Files)
            f.Create("./", raw);
        foreach (var f in Folders)
            f.Create("./", raw);

        Config.Current.TVars = null;
    }

    public static Template LoadTemplate(string name)
    {
        Template t = new(name);
        List<TemplateFile> files = new();
        DirectoryInfo d = new("./");
        foreach (var f in d.EnumerateFiles())
        {
            if (f.Name == "'.json")
            {
                t.Variables = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(f.Name)) ?? new();
                continue;
            }
            files.Add(new(f));
        }
        t.Files = files.ToArray();
        t.Folders = new DirectoryInfo("./").EnumerateDirectories().Select(p => new TemplateFolder(p)).ToArray();
        return t;
    }
}

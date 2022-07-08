﻿using Newtonsoft.Json;

namespace MakeMake;

[JsonObject(MemberSerialization.OptIn)]
internal class TemplateFolder
{
    [JsonProperty(Required = Required.Always)]
    public string Name { get; set; }

    [JsonProperty]
    public bool Parse { get; set; } = false;

    [JsonProperty(Required = Required.DisallowNull)]
    public TemplateFile[] Files { get; private set; } = Array.Empty<TemplateFile>();

    [JsonProperty(Required = Required.DisallowNull)]
    public TemplateFolder[] Folders { get; private set; } = Array.Empty<TemplateFolder>();

    [JsonConstructor]
    public TemplateFolder(string name)
    {
        Name = name;
    }

    public TemplateFolder(DirectoryInfo dir)
    {
        if (dir.Name.StartsWith('\''))
        {
            Parse = true;
            Name = dir.Name[1..];
        }
        else
            Name = dir.Name;
        Files = dir.EnumerateFiles().Select(p => new TemplateFile(p)).ToArray();
        Folders = dir.EnumerateDirectories().Select(p => new TemplateFolder(p)).ToArray();
    }

    public void Create(string directory)
    {
        string path = Path.Join(directory, Helpers.Parse(Name, '_', '(', ')'));
        Directory.CreateDirectory(path);
        foreach (var f in Files)
            f.Create(path);
        foreach (var d in Folders)
            d.Create(path);
    }
}

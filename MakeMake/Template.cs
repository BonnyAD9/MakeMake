﻿using Newtonsoft.Json;

namespace MakeMake;

[JsonObject(MemberSerialization.OptIn)]
internal class Template
{
    [JsonProperty(Required = Required.Always)]
    public string Name { get; set; }
    [JsonProperty(Required = Required.DisallowNull)]
    public string Description { get; set; } = "";
    //public string Usage { get; set; } = "";
    [JsonProperty(Required = Required.DisallowNull)]
    public TemplateFile[] Files { get; private set; } = Array.Empty<TemplateFile>();
    [JsonProperty(Required = Required.DisallowNull)]
    public TemplateFolder[] Folders { get; private set; } = Array.Empty<TemplateFolder>();

    public Template(string name)
    {
        Name = name;
    }

    public void Create()
    {
        foreach (var f in Files)
            f.Create("./");
        foreach (var f in Folders)
            f.Create("./");
    }
}
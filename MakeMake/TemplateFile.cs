using Newtonsoft.Json;
using System.Text;

namespace MakeMake;

[JsonObject(MemberSerialization.OptIn)]
internal class TemplateFile
{
    [JsonProperty(Required = Required.Always)]
    public string Name { get; set; }
    [JsonProperty]
    public bool Parse { get; set; } = false;
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
        File.WriteAllLines(Path.Join(directory, Name), Parse ? Contents.Select(p => ParseLine(p)) : Contents);
    }

    private static string ParseLine(string line)
    {
        StringBuilder sb = new(line.Length);
        for (int i = 0; i < line.Length; i++)
        {
            switch (line[i])
            {
                case '$':
                    if (i + 1 >= line.Length || line[i + 1] != '{')
                        goto default;
                    i += 2;
                    if (i >= line.Length)
                        return sb.ToString();
                    if (line[i] == '\'')
                    {
                        for (i++; i < line.Length && line[i] != '\''; i++)
                        {
                            switch (line[i])
                            {
                                case '\\':
                                    if (++i >= line.Length)
                                        return sb.ToString();
                                    goto default;
                                default:
                                    sb.Append(line[i]);
                                    continue;
                            }
                        }
                        i++;
                        continue;
                    }
                    StringBuilder sb2 = new();
                    for (; i < line.Length && line[i] != '}'; i++)
                        sb2.Append(line[i]);
                    sb.Append(GetVar(sb2.ToString()));
                    continue;
                default:
                    sb.Append(line[i]);
                    continue;
            }
        }
        return sb.ToString();
    }

    private static string GetVar(string name) => name switch
    {
        "Compiler" => Config.Current.Compiler,
        "DebugFlags" => Config.Current.DebugFlags,
        "ReleaseFlags" => Config.Current.ReleaseFlags,
        "OutName" => Config.Current.OutName ?? Config.Current.MainName,
        "MainName" => Config.Current.MainName,
        "Extension" => Config.Current.Extension,
        _ => $"${{{name}}}",
    };
}

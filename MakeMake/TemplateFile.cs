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
        if (file.Name.StartsWith('\''))
        {
            Parse = true;
            Name = file.Name[1..];
        }
        else
            Name = file.Name;
        Contents = File.ReadAllLines(file.FullName);
    }

    public void Create(string directory, bool raw = false)
    {
        if (raw)
        {
            File.WriteAllLines(Path.Join(directory, Parse ? "'" + Name : Name), Contents);
            return;
        }

        if (!Parse)
        {
            File.WriteAllLines(Path.Join(directory, Name), Contents);
            return;
        }

        File.WriteAllText(Path.Join(directory, Helpers.Parse(Name, '_', '(', ')', '_')), Helpers.Parse(Contents.Aggregate(new StringBuilder(), (sb, s) => sb.AppendLine(s)).ToString()));
    }
}

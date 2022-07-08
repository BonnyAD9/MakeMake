using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeMake;

internal static class Helpers
{
    public static string Parse(string line, char s = '$', char o = '{', char c = '}')
    {
        StringBuilder sb = new(line.Length);
        for (int i = 0; i < line.Length; i++)
        {
            if (line[i] == s && i + 1 < line.Length && line[i + 1] == o)
            {
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
                for (; i < line.Length && line[i] != c; i++)
                    sb2.Append(line[i]);
                sb.Append(GetVar(sb2.ToString()));
                continue;
            }
            sb.Append(line[i]);
            continue;
        }
        return sb.ToString();
    }

    public static string GetVar(string name, string? def = null) => name switch
    {
        "Compiler" => Config.Current.Compiler,
        "DebugFlags" => Config.Current.DebugFlags,
        "ReleaseFlags" => Config.Current.ReleaseFlags,
        "OutName" => Config.Current.OutName ?? Config.Current.MainName,
        "MainName" => Config.Current.MainName,
        "Extension" => Config.Current.Extension,
        _ => def ?? $"${{{name}}}",
    };
}

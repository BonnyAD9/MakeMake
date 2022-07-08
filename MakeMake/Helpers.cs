using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace MakeMake;

internal static class Helpers
{
    public static string Parse(string line, char s = '$', char o = '{', char c = '}', char e = '\\')
    {
        StringBuilder sb = new(line.Length);
        for (int i = 0; i < line.Length; i++)
        {
            if (line[i] == s && i + 1 < line.Length && line[i + 1] == o)
            {
                i += 2;
                sb.Append(GetVar(line, ref i, c, e));
                continue;
            }
            sb.Append(line[i]);
            continue;
        }
        return sb.ToString();
    }

    private static string GetVar(string str, ref int i, char endv = '}', char e = '\\')
    {
        string?[] strs = new string?[3];
        int strspos = 0;
        StringBuilder hsb = new();
        StringBuilder sb = new();
        string h;
        for (; i < str.Length && str[i] != endv; i++)
        {
            switch (str[i])
            {
                case '\'':
                    h = hsb.ToString();
                    sb.Append(GetVar(h, h));
                    hsb.Clear();
                    sb.Append(ReadStr(str, ref i, e));
                    continue;
                case ',':
                    StrsSet();
                    continue;
                default:
                    hsb.Append(str[i]);
                    continue;
            }
        }

        StrsSet();

        if (strs[0] is null)
            strs[0] = "";

        string? v = GetVar(strs[0]!);

        if (v is null)
        {
            return strs[2] is null ? strs[0]! : GetVar(strs[2]!, strs[2]!);
        }
        return strs[1] is null ? v : GetVar(strs[1]!, strs[1]!);

        void StrsSet()
        {
            sb.Append(hsb);
            hsb.Clear();
            strs[strspos] = sb.Length == 0 ? null : sb.ToString();
            sb.Clear();
            if (strspos + 1 < 3)
                strspos++;
        }
    }

    private static string ReadStr(string str, ref int i, char e = '\\')
    {
        StringBuilder sb = new();
        for (i++; i < str.Length && str[i] != '\''; i++)
        {

            if (str[i] == e)
            {
                if (++i >= str.Length)
                    return sb.ToString();
                switch (str[i])
                {
                    case 'n':
                        sb.Append('\n');
                        continue;
                    case 'r':
                        sb.Append('\r');
                        continue;
                }
            }
            sb.Append(str[i]);
        }
        return sb.ToString();
    }

    [return: NotNullIfNotNull("def")]
    public static string? GetVar(string str, string? def = null)
    {
        string? r = Config.Current.RVars.GetOrDefault(str) ?? Config.Current.TVars?.GetOrDefault(str) ?? Config.Current.Variables.GetOrDefault(str, def);
        return r is null && str == "Name" ? Config.Current.Variables["MainName"] : r;
    }

    public static void SetIfEmpty<TKey, TVaule>(this Dictionary<TKey, TVaule> dic, TKey key, TVaule value) where TKey : notnull
    {
        if (!dic.ContainsKey(key))
            dic.Add(key, value);
    }

    [return: NotNullIfNotNull("def")]
    public static TValue? GetOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dic, TKey key, TValue? def = default) where TKey : notnull => dic.ContainsKey(key) ? dic[key] : def;
}

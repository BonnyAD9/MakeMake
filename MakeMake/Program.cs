using Bny.Console;

namespace MakeMake;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            NoArg();
            return;
        }

        string? tem = null;

        for (int i = 0; i < args.Length; i++)
        {
            int ind;
            switch (args[i])
            {
                case "-n" or "--new":
                    if (++i >= args.Length)
                    {
                        Term.FormLine(Term.brightRed, "error:", Term.reset, Term.brightYellow, " -n ", Term.reset, "expects name");
                        return;
                    }
                    ind = Config.Current.Templates.FindIndex(p => p.Name == args[i]);
                    if (ind != -1)
                    {
                        string? s;
                        do
                        {
                            Console.Write($"Template with name '{args[i]}' already exists. Do you want to overwrite it? [Y/n]: ");
                            s = Console.ReadLine()?.ToLower();
                        } while (s != "y" && s != "n" && s != "");

                        if (s == "n")
                            continue;
                        Config.Current.Templates[ind] = Template.LoadTemplate(args[i]);
                        Config.Save();
                        continue;
                    }
                    Config.Current.Templates.Add(Template.LoadTemplate(args[i]));
                    Config.Save();
                    continue;
                case "-r" or "--remove":
                    if (++i >= args.Length)
                    {
                        Term.FormLine(Term.brightRed, "error:", Term.brightMagenta, " -l ", Term.reset, "expects name");
                        return;
                    }
                    ind = Config.Current.Templates.FindIndex(p => p.Name == args[i]);
                    if (ind == -1)
                    {
                        Term.FormLine(Term.brightRed, "error:", Term.reset, "name ", Term.brightYellow, args[i], Term.reset, "doesn't exist.");
                        return;
                    }
                    Config.Current.Templates.RemoveAt(ind);
                    Config.Save();
                    continue;
                case "-h" or "--help":
                    Help();
                    continue;
                case "-l" or "--list":
                    foreach (var temp in Config.Current.Templates)
                        Term.FormLine(Term.brightYellow, $"{temp.Name,-10} ", Term.reset, temp.Description);
                    continue;
                default:
                    tem = args[i];
                    break;
            }
            break;
        }

        if (tem is null)
            return;

        var t = Config.Current.Templates.FirstOrDefault(p => p.Name == args[0]);
        if (t is null)
        {
            Console.WriteLine($"There is no template with the name '{args[0]}'");
            return;
        }

        t.Create();
    }

    static void NoArg()
    {
        var f = Directory.EnumerateFiles("./").FirstOrDefault(p => p.EndsWith(".c"));

        if (f is null)
        {
            Term.FormLine(Term.brightRed, "error:", Term.reset, " No C file found");
            return;
        }

        if (File.Exists("Makefile"))
        {
            string? s;
            do
            {
                Console.Write("Makefile already exists, do you want to overwrite it? [Y/n]: ");
                s = Console.ReadLine()?.ToLower();
            } while (s != "y" && s != "n" && s != "");
            
            if (s == "n")
                return;
        }

        FileInfo fi = new(f);

        string makefile =
            "CC:=" + Config.Current.Compiler + "\n" +
            "OUT:=" + (Config.Current.OutName ?? fi.Name[..^2]) + Config.Current.Extension + "\n" +
            "CFLAGS:=" + Config.Current.DebugFlags + "\n" +
            "RFLAGS:=" + Config.Current.ReleaseFlags + "\n" +
            "CFILES:=" + fi.Name + "\n" +
            "\n" +
            "debug: $(CFILES)\n" +
            "\t$(CC) $(CFLAGS) -o $(OUT) $(CFILES)\n" +
            "\n" +
            "release: $(CFILES)\n" +
            "\t$(CC) $(RFLAGS) -o $(OUT) $(CFILES)\n" +
            "\n" +
            "clean:\n" +
            "\tdel $(OUT)\n";

        File.WriteAllText("Makefile", makefile);
    }

    static void Help()
    {
        Term.FormLine(
            "Wellcome to ", Term.italic, "MakeMake", Term.reset, " help\n",
            "Usage: ", Term.brightWhite, "makemake ", Term.brightBlack, "[flags] [template name]", Term.reset, "\n\n",
            "Flags:\n",
            Term.brightYellow, "  -h  --help", Term.reset, ": shows this help\n",
            Term.brightYellow, "  -n  --new", Term.brightWhite, " [template name]", Term.reset, ": creates new template\n",
            Term.brightYellow, "  -r  --remove", Term.brightWhite, " [template name]", Term.reset, ": removes template\n",
            Term.brightYellow, "  -l  --list", Term.reset, ": lists all templates\n"
            );
    }
}
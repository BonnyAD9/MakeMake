using Bny.Console;
using System.Reflection;

namespace MakeMake;

class Program
{
    static void Main(string[] args)
    {
        //args = new[] { "test" };

        if (args.Length == 0)
        {
            Help();
            return;
        }

        string? tem = null;
        bool raw = false;

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
                        Term.FormLine(Term.brightRed, "error:", Term.brightMagenta, " -r ", Term.reset, "expects name");
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
                case "-e" or "--edit":
                    raw = true;
                    continue;
            }
            if (args[i].StartsWith("-D"))
            {
                if (!args[i].Contains('='))
                {
                    Config.Current.RVars.TryAdd(args[i][2..], "");
                    continue;
                }
                var spl = args[i].Split('=', StringSplitOptions.None);
                Config.Current.RVars.TryAdd(spl[0][2..], spl[1]);
                continue;
            }
            tem = args[i];
        }

        if (tem is null)
            return;

        if (Directory.GetFiles("./").Length != 0 || Directory.GetDirectories("./").Length != 0)
        {
            string? s;
            do
            {
                Console.Write($"The current directory isn't empty. Do you want to continue anyway? [Y/n]: ");
                s = Console.ReadLine()?.ToLower();
            } while (s != "y" && s != "n" && s != "");

            if (s == "n")
                return;
        }

        var t = Config.Current.Templates.FirstOrDefault(p => p.Name == tem);
        if (t is null)
        {
            Console.WriteLine($"There is no template with the name '{args[0]}'");
            return;
        }

        t.Create(raw);
    }

    static void Help()
    {
        Term.FormLine(
            "Wellcome to help for ", Term.italic, Term.brightGreen, "MakeMake", Term.reset, " by ", string.Concat("BonnyAD9".Select((p, i) => Term.PrepareSB(Term.fg, 250 - 10 * i, 50, 170 + 10 * i, p).ToString())), Term.reset, " \n",
            Term.brightGreen, "Version: ", Term.brightYellow, Assembly.GetExecutingAssembly().GetName().Version!, Term.reset, "\n\n",
            Term.brightGreen, "Usage: ", Term.brightWhite, "makemake ", Term.brightBlack, "[flags] [template name] [flags]", Term.reset, "\n\n",
            Term.brightGreen, "Flags:", Term.reset, "\n",
            Term.brightYellow, "  -h  --help", Term.reset, "\n",
            "    shows this help\n\n",
            Term.brightYellow, "  -n  --new", Term.brightWhite, " [template name]", Term.reset, "\n",
            "    creates new template\n\n",
            Term.brightYellow, "  -r  --remove", Term.brightWhite, " [template name]", Term.reset, "\n",
            "    removes template\n\n",
            Term.brightYellow, "  -l  --list", Term.reset, "\n",
            "    lists all templates\n\n",
            Term.brightYellow, "  -D", Term.brightWhite, "[variable name]", Term.brightBlack, "=[value]", Term.reset, "\n",
            "    defines / redefines variable to the value\n\n",
            Term.brightYellow, "  -e  --edit", Term.reset, "\n",
            "    loads the template source\n\n",
            "For info about how to create templates see ", Term.brightBlue, "https://github.com/BonnyAD9/MakeMake#variables-in-files", Term.reset
            );
    }
}
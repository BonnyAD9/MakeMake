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
            Console.WriteLine("No C file found");
            return;
        }

        if (File.Exists("Makefile"))
        {
            string? s;
            do
            {
                Console.Write("Makefile already exists, do you want to override it? [Y/n]");
                s = Console.ReadLine()?.ToLower();
            } while (s != "y" || s != "n");
            
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
}
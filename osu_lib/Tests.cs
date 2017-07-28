using System;
using System.IO;
using System.Diagnostics;

using osu_lib;

public static class Tests
{
    //Sanity test that simply parses the beatmaps given
    //Can be individual files, a directory, or a mix
    public static void StandardParsing(string[] args)
    {
        if(args.Length == 0)
        {
            Console.WriteLine("Please enter beatmap filename (or drag onto application)");
            Console.WriteLine("Entering a directory will recursively parse all .osu files");
        }
        else
        {
            Stopwatch timer = new Stopwatch();

            foreach(string name in args)
            {
                if(File.Exists(name))
                {
                    ParseFile(name);
                }
                else if(Directory.Exists(name))
                {
                    ParseDirectory(name);
                }
                else
                {
                    Console.WriteLine("Error, " + name + " is not a file or directory");
                }
            }
        }
    }

    internal static void ParseDirectory(string path)
    {
        foreach(string filename in Directory.GetFiles(path, "*.osu", SearchOption.AllDirectories))
        {
            ParseFile(filename);
        }
    }

    internal static void ParseFile(string filename)
    {
        Stopwatch timer = new Stopwatch();
        try
        {
            timer.Start();
            HitPointParser parser = new HitPointParser(filename);
            timer.Stop();
            Console.WriteLine(parser.GetMetadata("title") + ": " + parser.GetMetadata("version"));
            Console.WriteLine("Parsed in " + timer.ElapsedMilliseconds + " ms");
        }
        catch(ArgumentException e)
        {
            Console.WriteLine(e);
        }
    }
}

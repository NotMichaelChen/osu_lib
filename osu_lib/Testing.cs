using System;
using System.Diagnostics;

using osu_lib;

public class Testing
{
    public static void Main(string[] args)
    {
        if(args.Length == 0)
        {
            Console.WriteLine("Please enter beatmap filename (or drag onto application)");
            return;
        }

        Stopwatch timer = new Stopwatch();

        foreach(string name in args)
        {
            timer.Start();
            HitPointParser parser = new HitPointParser(name);
            timer.Stop();

            Console.WriteLine(parser.GetMetadata("title") + ": " + parser.GetMetadata("version"));
            Console.WriteLine("Parsed in " + timer.ElapsedMilliseconds + " ms");
        }
    }
}
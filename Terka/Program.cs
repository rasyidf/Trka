using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terka.Parsing.Tokenizers;
using static System.Console;

namespace Terka
{
    internal class Program
    {
        private static string[] testcases = {
                "The.Young.Karl.Marx.2017.1080p.BluRay.x264-[YTS.AM].mkv",
                "Toy Story[HDTV 720p English - Spanish][1995].mkv",
                "El.dia.de.la.bestia.DVDrip.Spanish.DivX.by.Artik[SEDG].avi",
                "Blade.Runner.(1982).(Director's.Cut).CD1.DVDRip.XviD.AC3-WAF.avi" };

        private static void Main(string[] args)
        {
            var Matches = new List<Rebulk.Match>();
            Console.Title = "Moviebase - Terka";
            //Tokenizertest()
            WriteLine("Initialize Rules");
            var testcase = "2015 1254 1080 1235 2001 2008 1879";
            foreach (var item in Rebulk.RuleBase.GetRules())
            {
                WriteLine($"{item.ToString()} : {item.RB.Name} : {item.RB.Patterns?.Count}");

                Matches.AddRange(item.Match(testcase));
            }
            WriteLine($"Parsing {testcase}");
            WriteLine("Display Matches");
            foreach (var item in Matches)
            {
                WriteLine(item.ToString());
            }

            ReadKey();
        }

#pragma warning disable S1144 // Unused private types or members should be removed

        private static void Tokenizertest()
        {
            var a = new Tokenizer();

            foreach (var str in testcases)
            {
                Console.WriteLine(str);
                var b = a.Tokenize(str);
                foreach (var item in b)
                {
                    Console.WriteLine(item.ToString());
                }

                Console.WriteLine();
            }
        }

#pragma warning restore S1144 // Unused private types or members should be removed
    }
}
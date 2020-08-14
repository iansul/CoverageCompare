using CommandLine;
using Engine;
using Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CoverageCompare
{
    class Program
    {
        class Options
        {
            [Option(HelpText ="Left side of comparison")]
            public CoverageFile Left { get; set; }
            
            [Option(HelpText = "Rigth side of comparison")]
            public CoverageFile Right { get; set; }

            [Option(HelpText = "Type of comparison")]
            public CompareBy Comparison { get; set; }

            [Option(HelpText = "Show items exclusive to container")]
            public bool ShowExclusiveItems { get; set; }
        }
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed<Options>(o =>
                {
                    var cmp = new CompareFiles(o.Left, o.Right, o.Comparison);
                    Console.WriteLine($" Left file \"{Path.GetFileName(o.Left.FilePath)}\" coverage = {AggregateCoverage(o.Left.Modules, o.Comparison)}");
                    Console.WriteLine($"Right file \"{Path.GetFileName(o.Right.FilePath)}\" coverage = {AggregateCoverage(o.Right.Modules, o.Comparison)}");
                    RenderComparison(cmp, "", o.ShowExclusiveItems);
                });
        }

        static double AggregateCoverage(IEnumerable<ICoverageComparable> comparables, CompareBy compareBy)
        {
            double coverage = 0;
            var totalCovered = 0.0;
            var totalNotCovered = 0.0;
            bool blocks = false;
            bool covered = false;
            bool percentage = false;
            switch (compareBy)
            {
                case CompareBy.BlocksNotCovered:
                    blocks = true;
                    break;
                case CompareBy.BlocksCovered:
                    blocks = true;
                    covered = true;
                    break;
                case CompareBy.BlocksNotCoveredPercentage:
                    blocks = true;
                    percentage = true;
                    break;
                case CompareBy.BlocksCoveredPercentage:
                    blocks = true;
                    covered = true;
                    percentage = true;
                    break;
                case CompareBy.LinesNotCovered:
                    break;
                case CompareBy.LinesCovered:
                    covered = true;
                    break;
                case CompareBy.LinesNotCoveredPercentage:
                    percentage = true;
                    break;
                case CompareBy.LinesCoveredPercentage:
                    covered = true;
                    percentage = true;
                    break;
            }
            foreach (var c in comparables)
            {
                if (blocks)
                {
                    totalNotCovered += c.BlocksNotCovered;
                    totalCovered += c.BlocksCovered;
                }
                else
                {
                    totalNotCovered += c.LinesNotCovered;
                    totalCovered += c.LinesCovered;
                }
            }
            coverage = covered ? totalCovered : totalNotCovered;
            if (percentage)
            {
                coverage /= totalCovered + totalNotCovered;
                coverage *= 100;
            }
            return coverage;
        }

        private static void RenderComparison(Comparison c, string indent, bool showExclusiveItems)
        {
            foreach (var d in c.InnerComparablesWithDifferentCoverage)
            {
                var inequality = d.delta > 0 ? "<" : ">"; 
                Console.WriteLine($"{indent}Coverage for {d.innerComparableFromLeft.Name} differs between left and right files {c.CompareValue(d.innerComparableFromLeft)} {inequality} {c.CompareValue(d.innerComparableFromRight)}.");
                RenderComparison(new Comparison(d.innerComparableFromLeft, d.innerComparableFromRight, c.ComparisonType), indent + "  ", showExclusiveItems);
            }
            if (showExclusiveItems)
            {
                if (c.InnerComparablesExclusiveToLeft?.Any() == true)
                {
                    Console.WriteLine($"{indent}Coverage for these items exists only in left file");
                    foreach (var d in c.InnerComparablesExclusiveToLeft)
                    {
                        Console.WriteLine($"{indent} {d.Name}");
                    }
                }

                if (c.InnerComparablesExclusiveToRight?.Any() == true)
                {
                    Console.WriteLine($"{indent}Coverage for these items exists only in right file");
                    foreach (var d in c.InnerComparablesExclusiveToRight)
                    {
                        Console.WriteLine($"{indent} {d.Name}");
                    }
                }
            }
        }
    }
}

using System.Diagnostics;

namespace Advent_of_Code;

internal static class Program
{
    private static void Main(string[] args)
    {
        var totalElapsed = 0L;
        var day = args.Length > 1 && int.TryParse(args[1], out var single) ? single : 0;
        foreach (var (puzzle, parts) in IAdventPuzzle.GetPuzzles(args[0], day))
        {
            for (int part = 1; part <= parts; part++)
            {
                var solver = IAdventPuzzle.Solver(puzzle, part);

                using var sample = InputHelper.Create(puzzle, "sample");
                if (sample is not null)
                {
                    var sampleResult = solver.Solve(sample);
                    Console.WriteLine($"{puzzle.Name} part{part}: SAMPLE = {sampleResult}");
                }

                using var input = InputHelper.Create(puzzle);
                if (input is not null)
                {
                    var timer = Stopwatch.StartNew();
                    var result = solver.Solve(input);
                    timer.Stop();
                    totalElapsed += timer.ElapsedMilliseconds;
                    Console.WriteLine($"{puzzle.Name} part{part}: {result} in {timer.ElapsedMilliseconds}ms");
                }
            }
        }
        Console.WriteLine($"Total elapsed: {totalElapsed}ms");
    }
} 
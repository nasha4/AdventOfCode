using System.Diagnostics;

namespace Advent_of_Code;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        var totalElapsed = 0L;
        foreach (var puzzle in IAdventPuzzle.GetPuzzles(nameof(Advent2024)))
        {
            for (int part = 1; part <= (puzzle.Name == "Day25" ? 1 : 2); part++)
            {
                var solver = IAdventPuzzle.Solver(puzzle, part);

                using var sample = await InputHelper.Create(puzzle, "sample");
                if (sample is not null)
                {
                    var sampleResult = solver.Solve(sample);
                    Console.WriteLine($"{puzzle.Name} part{part}: SAMPLE = {sampleResult}");
                }

                using var input = await InputHelper.Create(puzzle);
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

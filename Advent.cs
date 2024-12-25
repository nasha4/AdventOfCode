using System.Diagnostics;

namespace Advent_of_Code;

internal static class Program
{
    const string InputPath = @"C:\Users\NashA4\source\repos\Advent of Code\Advent2024\input\";
    private static void Main(string[] args)
    {
        var totalElapsed = 0L;
        for (var day = 1; File.Exists($"{InputPath}{day:D2}.txt"); day++)
        {
            for (int part = 1; part <= (day == 25 ? 1 : 2); part++)
            {
                if (File.Exists($"{InputPath}sample{day:D2}.txt"))
                {
                    using var sample = new InputHelper(new($"{InputPath}sample{day:D2}.txt"));
                    Console.WriteLine($"Sample {day} part {part}: {PuzzleFactory(day, sample).Solve(part == 1)}");
                }
                using var helper = new InputHelper(new($"{InputPath}{day:D2}.txt"));
                var puzzle = PuzzleFactory(day, helper);
                var timer = Stopwatch.StartNew();
                var result = puzzle.Solve(part == 1);
                timer.Stop();
                totalElapsed += timer.ElapsedMilliseconds;
                Console.WriteLine($"Day {day} part {part}: {result} in {timer.ElapsedMilliseconds}ms");
            }
        }
        Console.WriteLine($"Total elapsed: {totalElapsed}ms");
    }
    private static IAdventPuzzle PuzzleFactory(int day, InputHelper helper)
    {
        var type = Type.GetType($"Advent_of_Code.Advent2024.Day{day:D2}")
            ?? throw new ArgumentException($"Could not find type Day{day:D2}");
        return Activator.CreateInstance(type, helper) as IAdventPuzzle
            ?? throw new ArgumentException($"Could not create {nameof(IAdventPuzzle)} from type Day{day:D2}");
    }
}

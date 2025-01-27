namespace Advent_of_Code.Advent2024;

public class Day02(bool isPart1) : IAdventPuzzle
{
    public string Solve(InputHelper inputHelper)
    {
        var levels = inputHelper.EachLine(line => line.Split(' ').Select(int.Parse));

        return isPart1
            ? levels.Count(IsSafe).ToString()
            : levels
                .Select(x => Enumerable.Range(0, x.Count() + 1)
                    .Select(n => x.Take(n).Concat(x.Skip(n + 1))))
                .Count(x => x.Any(IsSafe)).ToString();
    }

    private static bool IsSafe(IEnumerable<int> level)
    {
        var deltas = level.Zip(level.Skip(1), (a, b) => a - b);
        return deltas.All(d => d >= 1 && d <= 3)
            || deltas.All(d => d >= -3 && d <= -1);
    }
}
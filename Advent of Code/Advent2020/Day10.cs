namespace Advent_of_Code.Advent2020;

public class Day10(bool isPart1) : IAdventPuzzle
{
    public string Solve(InputHelper inputHelper)
    {
        var jolts = inputHelper.EachLine(int.Parse).ToHashSet();
        var maxJoltage = jolts.Max() + 3;
        jolts.UnionWith([0, maxJoltage]);

        if (isPart1)
        {
            var sortedJolts = jolts.Order();
            var gapCount = sortedJolts.Zip(sortedJolts.Skip(1), (a, b) => b - a)
                .CountBy(x => x).ToDictionary();

            return $"{gapCount[1] * gapCount[3]}";
        }

        pathsFromMemo[maxJoltage] = 1;

        return PathsFrom(0, jolts).ToString();
    }

    private readonly Dictionary<int, long> pathsFromMemo = [];
    private long PathsFrom(int joltage, IReadOnlySet<int> jolts)
    {
        if (pathsFromMemo.TryGetValue(joltage, out var paths))
            return paths;
        else 
            return pathsFromMemo[joltage] = jolts.Contains(joltage)
                ? Enumerable.Range(joltage + 1, 3).Sum(j => PathsFrom(j, jolts)) : 0;
    }
}

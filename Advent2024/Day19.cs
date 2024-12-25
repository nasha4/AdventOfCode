namespace Advent_of_Code.Advent2024;

public class Day19(InputHelper inputHelper) : IAdventPuzzle
{
    public long Solve(bool isPart1)
    {
        var towelsByLength = inputHelper.EachLineInSection(line => line).Single().Split(", ")
            .GroupBy(x => x.Length).ToDictionary(g => g.Key, g => g.ToHashSet());

        var matches = inputHelper.EachLineInSection(line => line).Select(design =>
            Enumerable.Range(0, design.Length).Select(start =>
                towelsByLength.Keys.Where(length =>
                    start + length <= design.Length &&
                    towelsByLength[length].Contains(design.Substring(start, length)))
                .ToHashSet())
            .Aggregate(Enumerable.Repeat(0L, design.Length).Prepend(1).ToArray(), (accumulator, substrings) =>
                accumulator[1..].Select((n, i) => substrings.Contains(i + 1) ? n + accumulator[0] : n).ToArray())[0]);

        return isPart1 ? matches.Count(x => x > 0) : matches.Sum();
    }
}

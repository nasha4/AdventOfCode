using System.Text.RegularExpressions;

namespace Advent_of_Code.Advent2022;

public partial class Day04(bool isPart1) : IAdventPuzzle
{
    public string Solve(InputHelper inputHelper)
    {
        var ranges = inputHelper.EachMatchGroup(Assignments, matches => matches[1..].Select(int.Parse).ToArray());

        return isPart1
            ? ranges.Count(r => r[0] <= r[2] && r[3] <= r[1] || r[2] <= r[0] && r[1] <= r[3]).ToString()
            : ranges.Count(r => r[1] >= r[2] && r[0] <= r[3]).ToString();
    }

    [GeneratedRegex(@"(\d+)-(\d+),(\d+)-(\d+)")]
    private static partial Regex Assignments { get; }
}
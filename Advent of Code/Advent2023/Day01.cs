using System.Text.RegularExpressions;

namespace Advent_of_Code.Advent2023;

public class Day01(bool isPart1) : IAdventPuzzle
{
    private Dictionary<string, int> Value { get; } =
        (isPart1 ? [] : new[] { "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" })
            .Select((x, i) => (name: x, value: i + 1))
            .Concat("0123456789".Select((x, i) => (name: x.ToString(), value: i)))
            .ToDictionary(p => p.name, p => p.value);

    public string Solve(InputHelper inputHelper) => inputHelper
        .EachLine(line => (first: Regex.Match(line, string.Join('|', Value.Keys)), last: Regex.Match(line, string.Join('|', Value.Keys), RegexOptions.RightToLeft)))
        .Sum(x => Value[x.first.Value] * 10 + Value[x.last.Value]).ToString();
}

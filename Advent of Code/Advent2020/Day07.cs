using System.Text.RegularExpressions;

namespace Advent_of_Code.Advent2020;

public partial class Day07(bool isPart1) : IAdventPuzzle
{
    public string Solve(InputHelper inputHelper)
    {
        var adj = inputHelper.EachLine(BagPattern.Match)
            .SelectMany(match => match.Groups[2].Captures.Zip(match.Groups[3].Captures, (a, b) => (a: int.Parse(a.Value), b: b.Value)),
                (m, ab) => (bag: m.Groups[1].Value, n: ab.a, contents: ab.b));

        return isPart1
            ? ContainedBy("shiny gold", adj.GroupBy(x => x.contents, x => x.bag).ToDictionary(g => g.Key, g => g.AsEnumerable())).Count().ToString()
            : Contains("shiny gold", adj.GroupBy(x => x.bag).ToDictionary(g => g.Key, g => g.ToDictionary(x => x.contents, x => x.n))).ToString();
    }

    private static IEnumerable<string> ContainedBy(string bag, Dictionary<string, IEnumerable<string>> containedBy) =>
        containedBy.TryGetValue(bag, out var containers) ? containers.Aggregate(containers, (bags, container) => bags.Union(ContainedBy(container, containedBy))) : [];

    private static int Contains(string bag, Dictionary<string, Dictionary<string, int>> contains) =>
        contains.TryGetValue(bag, out var contents) ? contents.Sum(p => p.Value) + contents.Sum(p => p.Value * Contains(p.Key, contains)) : 0;

    [GeneratedRegex(@"^(.+) bags contain(?: (\d+) (.+?) bags?[,.])*$")]
    private static partial Regex BagPattern { get; }
}
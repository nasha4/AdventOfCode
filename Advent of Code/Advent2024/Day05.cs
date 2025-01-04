namespace Advent_of_Code.Advent2024;

public class Day05(bool isPart1) : IAdventPuzzle
{
    private sealed class PageComparer<T>(IReadOnlySet<(T?, T?)> rules) : IComparer<T>
    {
        public int Compare(T? x, T? y)
        {
            if (rules.Contains((x, y))) return -1;
            if (rules.Contains((y, x))) return 1;
            return 0;
        }
        public bool IsSorted(IEnumerable<T> update) => update.Zip(update.Skip(1), Compare).All(d => d <= 0);
    }
    public string Solve(InputHelper inputHelper)
    {
        var rules = inputHelper.EachLineInSection(line => line.Split('|').Select(int.Parse))
            .Select(x => (x.First(), x.Last())).ToHashSet();

        var updates = inputHelper.EachLineInSection(line => line.Split(',').Select(int.Parse));

        var comparer = new PageComparer<int>(rules);

        return isPart1
            ? updates.Where(comparer.IsSorted).Sum(u => u.ToArray()[u.Count() / 2]).ToString()
            : updates.Where(u => !comparer.IsSorted(u)).Sum(u => u.Order(comparer).ToArray()[u.Count() / 2]).ToString();
    }
}

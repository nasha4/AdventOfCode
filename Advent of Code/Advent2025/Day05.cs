namespace Advent_of_Code.Advent2025;

public class Day05(bool isPart1) : IAdventPuzzle
{
    public string Solve(InputHelper inputHelper)
    {
        var ranges = inputHelper.EachLineInSection(line => line.Split('-').Select(long.Parse).ToArray());
        if (isPart1)
        {
            var isFresh = ranges.Select(range => (lo: range[0], hi: range[1])).ToList();
            return inputHelper.EachLineInSection(long.Parse).Count(x => isFresh.Any(f => f.lo <= x && x <= f.hi)).ToString();
        }

        var merge = new HashSet<(long lo, long hi)>();
        foreach (var (lo, hi) in ranges.Select(x => (x[0], x[1])))
        {
            var overlap = merge.Where(x =>
                x.lo <= lo && lo <= x.hi ||
                x.lo <= hi && hi <= x.hi ||
                lo <= x.lo && x.lo <= hi).Append((lo, hi)).ToList();
            merge.ExceptWith(overlap);
            merge.Add((overlap.Min(x => x.lo), overlap.Max(x => x.hi)));
        }
        return merge.Sum(x => x.hi - x.lo + 1).ToString();
    }
}
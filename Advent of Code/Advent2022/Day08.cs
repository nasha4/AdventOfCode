namespace Advent_of_Code.Advent2022;

public class Day08(bool isPart1) : IAdventPuzzle
{
    public string Solve(InputHelper inputHelper)
    {
        var trees = new Grid.Helper<int>(inputHelper.EachLine(line => line.Select(c => c - '0')));

        return isPart1
            ? "NSEW".Aggregate(Enumerable.Empty<int[]>(),
                (vis, orientation) => vis.Union(CheckVisibility(trees, orientation), trees)).Count().ToString()
            : trees.Points.Max(p => ScenicScore(trees, p[1], p[0])).ToString();
    }

    private static IEnumerable<int[]> CheckVisibility(Grid.Helper<int> trees, char orientation)
    {
        var (aRange, bRange) = (trees.Ranges[0], trees.Ranges[1]);
        if ("EW".Contains(orientation)) // vertical, so swap x and y (swap them back again later)
            (aRange, bRange) = (bRange, aRange);
        if ("ES".Contains(orientation)) // negative, so reverse the inner loop
            bRange = bRange.Reverse();

        foreach (var a in aRange)
        {
            var maxHeight = -1;
            foreach (var b in bRange)
            {
                var (x, y) = "EW".Contains(orientation) ? (a, b) : (b, a);
                if (trees[[y, x]] > maxHeight)
                {
                    maxHeight = trees[[y, x]];
                    yield return [y, x];
                }
            }
        }
    }
    private static int ScenicScore(Grid.Helper<int> trees, int x, int y)
    {
        int n, s, e, w; // look in each direction until we find a value >= our start tree (or find the edge)
        for (e = x + 1; e < trees.Max[1] && trees[[y, e]] < trees[[y, x]]; e++) ;
        for (w = x - 1; w > 0 && trees[[y, w]] < trees[[y, x]]; w--) ;
        for (s = y + 1; s < trees.Max[0] && trees[[s, x]] < trees[[y, x]]; s++) ;
        for (n = y - 1; n > 0 && trees[[n, x]] < trees[[y, x]]; n--) ;
        return (y - n) * (y - s) * (x - e) * (x - w);
    }

}

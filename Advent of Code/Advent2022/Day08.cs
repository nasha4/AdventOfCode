namespace Advent_of_Code.Advent2022;

public class Day08(bool isPart1) : IAdventPuzzle
{
    public string Solve(InputHelper inputHelper)
    {
        var trees = new GridHelper<int>(inputHelper.EachLine(line => line.Select(c => c - '0')));

        return isPart1
            ? "NSEW".Aggregate(Enumerable.Empty<int[]>(),
                (vis, orientation) => vis.Union(CheckVisibility(trees, orientation), trees)).Count().ToString()
            : trees.Points.Max(p => ScenicScore(trees, p[1], p[0])).ToString();
    }

    private static IEnumerable<int[]> CheckVisibility(GridHelper<int> trees, char orientation)
    {
        var aRange = Enumerable.Range(0, trees.Max[0]);
        var bRange = Enumerable.Range(0, trees.Max[1]);
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
    private static int ScenicScore(GridHelper<int> trees, int x, int y)
    {
        var (xMax, yMax) = (trees.Max[1] - 1, trees.Max[0] - 1);
        int n, s, e, w; // look in each direction until we find a value >= our start tree (or find the edge)
        for (e = x + 1; e < xMax && trees[[y, e]] < trees[[y, x]]; e++) ;
        for (w = x - 1; w > 0 && trees[[y, w]] < trees[[y, x]]; w--) ;
        for (s = y + 1; s < yMax && trees[[s, x]] < trees[[y, x]]; s++) ;
        for (n = y - 1; n > 0 && trees[[n, x]] < trees[[y, x]]; n--) ;
        return (y - n) * (y - s) * (x - e) * (x - w);
    }

}

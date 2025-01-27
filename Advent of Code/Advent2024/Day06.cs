namespace Advent_of_Code.Advent2024;

public class Day06(bool isPart1) : IAdventPuzzle
{
    private Grid.Helper grid = new();
    private Dictionary<int, HashSet<int>> wallsByRow = [];
    private Dictionary<int, HashSet<int>> wallsByCol = [];
    public string Solve(InputHelper inputHelper)
    {
        grid = new(inputHelper.EachLine());
        wallsByRow = grid['#'].GroupBy(p => p[0], p => p[1]).ToDictionary(g => g.Key, g => g.ToHashSet());
        wallsByCol = grid['#'].GroupBy(p => p[1], p => p[0]).ToDictionary(g => g.Key, g => g.ToHashSet());
        var guard0 = grid['^'].Single();

        var seen = Patrol(guard0, 0).SelectMany(ExpandStep).Distinct(grid);
        if (isPart1) return seen.Count().ToString();

        var loops = 0;
        foreach (var newWall in seen.Except([guard0]))
        {
            var (x, y) = (newWall[1], newWall[0]);
            if (!wallsByRow.ContainsKey(y)) wallsByRow[y] = [];
            if (!wallsByCol.ContainsKey(x)) wallsByCol[x] = [];
            wallsByRow[y].Add(x);
            wallsByCol[x].Add(y);
            if (Patrol(guard0, 0).Count == 0) loops++;
            wallsByRow[y].Remove(x);
            wallsByCol[x].Remove(y);
        }
        return loops.ToString();
    }

    private Dictionary<int[], Dictionary<int, int>> Patrol(int[] start, int dir0)
    {
        var steps = new Dictionary<int[], Dictionary<int, int>>(grid);
        var (guard, dir) = (start.ToArray(), dir0);
        while (grid[guard] > 0)
        {
            var n = Walk(guard, dir);
            if (!steps.TryGetValue(guard, out var tile))
                steps[[.. guard]] = new() { [dir] = n };
            else if (!tile.TryAdd(dir, n))
                return [];
            dir = (dir + 1) % 4;
        }
        return steps;
    }

    private int Walk(int[] guard, int dir)
    {
        int[] end = dir switch
        {
            0 => [wallsByCol.GetValueOrDefault(guard[1], []).Where(y => y < guard[0]).Order().LastOrDefault(grid.Min[0] - 2), guard[1]],
            2 => [wallsByCol.GetValueOrDefault(guard[1], []).Where(y => y > guard[0]).Order().FirstOrDefault(grid.Max[0] + 2), guard[1]],
            1 => [guard[0], wallsByRow.GetValueOrDefault(guard[0], []).Where(x => x > guard[1]).Order().FirstOrDefault(grid.Max[1] + 2)],
            _ => [guard[0], wallsByRow.GetValueOrDefault(guard[0], []).Where(x => x < guard[1]).Order().LastOrDefault(grid.Min[1] - 2)]
        };
        var n = Math.Abs(guard[0] - end[0] + guard[1] - end[1]);
        switch (dir)
        {
            case 0: guard[0] = end[0] + 1; return n - 1;
            case 1: guard[1] = end[1] - 1; return n - 1;
            case 2: guard[0] = end[0] - 1; return n - 1;
            default: guard[1] = end[1] + 1; return n - 1;
        }
    }

    private static IEnumerable<int[]> ExpandStep(KeyValuePair<int[], Dictionary<int, int>> step) =>
        step.Value.Keys.SelectMany(dir => Enumerable.Range(0, step.Value[dir])
            .Select(d => dir switch
            {
                0 => [step.Key[0] + d, step.Key[1]],
                2 => [step.Key[0] - d, step.Key[1]],
                1 => [step.Key[0], step.Key[1] - d],
                _ => new[] { step.Key[0], step.Key[1] + d },
            }));
}
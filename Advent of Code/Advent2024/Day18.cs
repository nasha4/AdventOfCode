namespace Advent_of_Code.Advent2024;

public class Day18(bool isPart1) : IAdventPuzzle
{
    public string Solve(InputHelper inputHelper)
    {
        var obstacles = inputHelper.EachLine(line => line.Split(',').Select(int.Parse).ToArray()).ToList();
        var grid = new Grid.Helper<bool>(obstacles[..1024].ToHashSet());
        
        if (isPart1) return FindPath(grid).ToString();

        var (min, max) = (1024, obstacles.Count);
        while (min < max)
        {
            var mid = (min + max) / 2;
            grid = new Grid.Helper<bool>(obstacles[..mid].ToHashSet(grid));
            if (FindPath(grid) < 0)
                max = mid - 1;
            else
                min = mid + 1;
        }
        return $"{obstacles[max - 1][0]},{obstacles[max - 1][1]}";
    }

    private static int FindPath(Grid.Helper<bool> grid)
    {
        var search = new PriorityQueue<int[], int>([([0, 0], 0)]);
        var reachable = new HashSet<int[]>(grid);
        while (search.TryDequeue(out var tile, out var score))
        {
            if (!reachable.Add(tile) || grid[tile]) continue;
            if (tile.SequenceEqual([70, 70])) return score;
            search.EnqueueRange(grid.Orthogonal(tile).Select(p => (p, score + 1)));
        }
        return -1;
    }
}
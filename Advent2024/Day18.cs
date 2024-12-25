namespace Advent_of_Code.Advent2024;

public class Day18(InputHelper inputHelper) : IAdventPuzzle
{
    public long Solve(bool isPart1)
    {
        var obstacles = inputHelper.EachLine(line => line.Split(',').Select(int.Parse).ToArray()).ToArray();

        var comparer = new Cartesian<int>.GridHelper();
        if (isPart1) return FindPath(obstacles[..1024].ToHashSet(comparer), new HashSet<int[]>(comparer));

        var (min, max) = (1024, obstacles.Length);
        while (min < max)
        {
            var mid = (min + max) / 2;
            if (FindPath(obstacles[..mid].ToHashSet(comparer), new HashSet<int[]>(comparer)) < 0)
                max = mid - 1;
            else
                min = mid + 1;
        }
        return obstacles[max - 1][0] * 1_000_000 + obstacles[max - 1][1];
    }

    private static int FindPath(HashSet<int[]> obstacles, HashSet<int[]> reachable)
    {
        var search = new PriorityQueue<int[], int>([([0, 0], 0)]);
        while (search.TryDequeue(out var tile, out var score))
        {
            if (!reachable.Add(tile)) continue;
            if (obstacles.Contains(tile)) continue;
            if (tile.SequenceEqual([70, 70])) return score;
            search.EnqueueRange(Cartesian<int>.Orthogonal(tile, [0, 0], [71, 71]).Select(p => (p, score + 1)));
        }
        return -1;
    }
}

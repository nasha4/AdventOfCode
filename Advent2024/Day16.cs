namespace Advent_of_Code.Advent2024;

public class Day16(InputHelper inputHelper) : IAdventPuzzle
{
    private readonly record struct Tile(int X, int Y, int Dir)
    {
        private static readonly (int x, int y)[] directions = [(1, 0), (0, -1), (-1, 0), (0, 1)];
        public readonly (int, int) XY => (X, Y);
        public readonly IEnumerable<(Tile tile, int score)> Next(int score) => [
            (this with { X = X + directions[Dir].x, Y = Y + directions[Dir].y }, score + 1),
            (this with { Dir = (Dir+1)%4 }, score + 1000),
            (this with { Dir = (Dir+3)%4 }, score + 1000) ];

        public readonly IEnumerable<Tile> Previous => [
            this with { X = X - directions[Dir].x, Y = Y - directions[Dir].y },
            this with { Dir = (Dir+1)%4 },
            this with { Dir = (Dir+3)%4 }];
    }

    public long Solve(bool isPart1)
    {
        var grid = inputHelper.EachLine(line => line)
            .SelectMany((line, y) => line.Select((c, x) => (c, x, y)))
            .GroupBy(t => t.c, t => (t.x, t.y))
            .ToDictionary(g => g.Key, g => g.ToHashSet());
        var (start, goal) = (grid['S'].Single(), grid['E'].Single());

        var reachable = new Dictionary<Tile, int>();
        var search = new PriorityQueue<Tile, int>([(new(start.x, start.y, 0), 0)]);

        var best = int.MaxValue;
        while (search.TryDequeue(out var tile, out var score) && score <= best)
        {
            if (tile.XY == goal)
            {
                if (isPart1) return score;
                best = score;
            }
            if (!reachable.ContainsKey(tile) && !grid['#'].Contains(tile.XY))
            {
                reachable[tile] = score;
                search.EnqueueRange(tile.Next(score));
            }
        }
        var bestPaths = new HashSet<(int x, int y)>();
        var backtrack = new Queue<Tile>(reachable.Keys.Where(s => s.XY == goal));
        while (backtrack.TryDequeue(out var tile))
        {
            bestPaths.Add(tile.XY);
            foreach (var previous in tile.Previous.Where(p => reachable.TryGetValue(p, out var reach) && reach < reachable[tile]))
                backtrack.Enqueue(previous);
        }
        return bestPaths.Count;
    }
}

namespace Advent_of_Code.Advent2022;

public class Day24(bool isPart1) : IAdventPuzzle
{
    private sealed class Blizzards
    {
        private readonly int[] Size;
        private readonly Dictionary<int, HashSet<Blizzard>> BlizzX = [], BlizzY = [];
        public Blizzards(int[] size, params IEnumerable<(int[] yx, char direction)> blizzards)
        {
            Size = size;
            foreach (var (yx, direction) in blizzards)
            {
                var (blizz, c) = direction is '<' or '>' ? (BlizzY, yx[0]) : (BlizzX, yx[1]);
                if (blizz.TryGetValue(c, out var set))
                    set.Add(new Blizzard(yx[1], yx[0], direction));
                else
                    blizz.Add(c, [new Blizzard(yx[1], yx[0], direction)]);
            }
        }
        private bool At(Blizzard b, int step, int[] yx) => b.Direction switch
        {
            '>' => yx[1] == (b.X - 1 + step) % Size[1] + 1,
            '<' => yx[1] == (b.X - 1 + step * (Size[1] - 1)) % Size[1] + 1,
            'v' => yx[0] == (b.Y - 1 + step) % Size[0] + 1,
            '^' => yx[0] == (b.Y - 1 + step * (Size[0] - 1)) % Size[0] + 1,
            _ => false
        };

        public bool AnyAt(int[] yx, int step) => (BlizzX.TryGetValue(yx[1], out var xSet) ? xSet : [])
            .Concat(BlizzY.TryGetValue(yx[0], out var ySet) ? ySet : [])
            .Any(b => At(b, step, yx));

        private readonly record struct Blizzard(int X, int Y, char Direction);
    }
    public string Solve(InputHelper inputHelper)
    {
        var grid = new Grid.Helper(inputHelper.EachLine());
        var blizzards = new Blizzards([grid.Max[0] - 1, grid.Max[1] - 1], "<>^v".SelectMany(c => grid[c], (c, yx) => (yx, c)));

        var start = grid['.'].Single(p => p[0] == grid.Min[0]);
        var goal = grid['.'].Single(p => p[0] == grid.Max[0]);

        var there = ShortestPath(start, 0, goal, grid, blizzards);
        if (isPart1) return there.ToString();
        var backAgain = ShortestPath(goal, there, start, grid, blizzards);
        return ShortestPath(start, backAgain, goal, grid, blizzards).ToString();
    }

    private static int ShortestPath(int[] start, int startStep, int[] goal, Grid.Helper grid, Blizzards blizzards)
    {
        var nextSearch = new HashSet<int[]>(grid) { start };
        for (var step = startStep; nextSearch.Count > 0; step++)
        {
            (var thisSearch, nextSearch) = (nextSearch, new(grid));

            foreach (var tile in thisSearch)
            {
                if (grid.Equals(tile, goal))
                    return step;
                if (grid[tile] != '#' && !blizzards.AnyAt(tile, step))
                {
                    nextSearch.UnionWith(grid.Orthogonal(tile));
                    nextSearch.Add(tile);
                }
            }
        }
        return -1;
    }
}
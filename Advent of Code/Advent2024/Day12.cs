namespace Advent_of_Code.Advent2024;

public class Day12(bool isPart1) : IAdventPuzzle
{
    public string Solve(InputHelper inputHelper)
    {
        var grid = new GridHelper(inputHelper.EachLine());
        var corners = new[] { 1, 0, 0, 1, 1, 0, 0, -1, -1, 0, 0, -1, -1, 0, 0, 1 }.Chunk(2).Chunk(2);

        var perimeters = new List<int>();
        var sides = new List<int>();
        var regions = new Dictionary<int[], int>(grid);
        foreach (var p in grid.Points)
        {
            if (regions.ContainsKey(p)) continue;
            perimeters.Add(0);
            sides.Add(0);
            var flood = new Queue<int[]>([p]);
            while (flood.TryDequeue(out var q))
            {
                if (regions.TryAdd(q, perimeters.Count - 1))
                {
                    var adj = grid.Orthogonal(q).Where(a => grid[a] == grid[q]);
                    adj.Where(a => !regions.ContainsKey(a)).ToList().ForEach(flood.Enqueue);
                    perimeters[^1] += 4 - adj.Count();
                    sides[^1] += corners.Count(corner => corner.All(c => grid[q] != grid[Cartesian<int>.VectorSum(q, c)]));
                    sides[^1] += corners.Count(corner => corner.All(c =>
                        grid[q] == grid[Cartesian<int>.VectorSum(q, c)] &&
                        grid[q] != grid[Cartesian<int>.VectorSum([q, .. corner])]));
                }
            }
        }

        return regions.GroupBy(kvp => kvp.Value, kvp => kvp.Key)
            .Sum(g => g.Count() * (isPart1 ? perimeters[g.Key] : sides[g.Key])).ToString();
    }
}

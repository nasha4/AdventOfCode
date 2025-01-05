namespace Advent_of_Code.Advent2024;

public class Day10(bool isPart1) : IAdventPuzzle
{
    public string Solve(InputHelper inputHelper)
    {
        var grid = new GridHelper<int>(inputHelper.EachLine(line => line.Select(c => c - '0')));
        var score = new Dictionary<int[], IEnumerable<int[]>>(grid);
        var rating = new Dictionary<int[], int>(grid);

        foreach (var p in grid[9])
        {
            score[p] = [p];
            rating[p] = 1;
        }
        for (var height = 8; height >= 0; height--)
        {
            foreach (var p in grid[height])
            {
                foreach (var q in grid.Orthogonal(p).Where(adj => grid[adj] == height + 1))
                {
                    score[p] = score.GetValueOrDefault(p, []).Union(score.GetValueOrDefault(q, []));
                    rating[p] = rating.GetValueOrDefault(p) + rating.GetValueOrDefault(q);
                }
            }
        }
        return grid[0].Sum(p => isPart1 ? score[p].Count() : rating[p]).ToString();
    }
}

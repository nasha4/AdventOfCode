﻿namespace Advent_of_Code.Advent2024;

public class Day10(InputHelper inputHelper) : IAdventPuzzle
{
    public long Solve(bool isPart1)
    {
        var grid = new Cartesian<int>.GridHelper<char, int>(inputHelper.EachLine(line => line), c => c - '0');
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
        return grid[0].Sum(p => isPart1 ? score[p].Count() : rating[p]);
    }
}

namespace Advent_of_Code.Advent2024;

public class Day04(bool isPart1) : IAdventPuzzle
{
    public string Solve(InputHelper inputHelper)
    {
        var grid = inputHelper.EachLine(line => line.ToList()).ToList();

        (int x, int y)[][] looks = isPart1
            ? [[(0, 0), (0, 1), (0, 2), (0, 3)], // ↓
               [(0, 0), (1, 1), (2, 2), (3, 3)], // ↘
               [(0, 0), (1, 0), (2, 0), (3, 0)], // →
               [(0, 0), (1,-1), (2,-2), (3,-3)]] // ↗
            : [[(0, 0), (0, 2), (1, 1), (2, 0), (2, 2)], // ><
               [(0, 0), (2, 0), (1, 1), (0, 2), (2, 2)]]; // ∨∧

        IEnumerable<string> matches = ["XMAS", "SAMX", "MMASS", "SSAMM"];
        var xmas = 0;
        for (var y = 0; y < grid.Count; y++)
        {
            for (var x = 0; x < grid[y].Count; x++)
            {
                xmas += looks
                    .Where(look => y + look[^1].y >= 0 && x + look[^1].x < grid[y].Count && y + look[^1].y < grid.Count)
                    .Select(look => new string(look.Select(d => grid[y + d.y][x + d.x]).ToArray()))
                    .Count(matches.Contains);
            }
        }
        return xmas.ToString();
    }
}

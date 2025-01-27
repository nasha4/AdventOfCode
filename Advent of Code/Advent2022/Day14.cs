namespace Advent_of_Code.Advent2022;

public class Day14(bool isPart1) : IAdventPuzzle
{
    public string Solve(InputHelper inputHelper)
    {
        var cave = new HashSet<(int x, int y)>();
        foreach (var line in inputHelper.EachLine(line => line.Split(" -> ").Select(pair => pair.Split(',').Select(int.Parse).ToArray())))
        {
            foreach (var (a, b) in line.Zip(line.Skip(1)))
            { // treat each pair of points as corners of a rectangle. one dimension will always just be 1
                var (lo, hi) = (a[0] > b[0] || a[1] > b[1]) ? (b, a) : (a, b);
                for (var x = lo[0]; x <= hi[0]; x++)
                    for (var y = lo[1]; y <= hi[1]; y++)
                        cave.Add((x, y));
            }
        }
        var caveDepth = cave.Max(p => p.y);

        for (var sandCount = 0; ; sandCount++)
        {
            var (sandX, sandY) = (500, 0);
            while (sandY < caveDepth || !isPart1)
            {
                if (!isPart1 && sandY > caveDepth)
                    break;
                else if (new[] { sandX, sandX - 1, sandX + 1 }.FirstOrDefault(x => !cave.Contains((x, sandY + 1))) is var x and > 0)
                    (sandX, sandY) = (x, sandY + 1);
                else
                    break;
            }
            if (sandY >= caveDepth && isPart1)
                return $"{sandCount}"; // don't count the last sand, it fell into the abyss
            else if (sandY == 0)
                return $"{sandCount + 1}"; // do count the last sand, it settled at the source
            else
                cave.Add((sandX, sandY));
        }
    }
}
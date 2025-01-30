using System.Text.RegularExpressions;

namespace Advent_of_Code.Advent2024;

public partial class Day14(bool isPart1) : IAdventPuzzle
{
    public string Solve(InputHelper inputHelper)
    {
        var (maxX, maxY, seconds) = (101, 103, 100);
        var robots = inputHelper.EachLine(line => Integer.Matches(line))
            .Select(ms => ms.Select(m => int.Parse(m.Value)).ToArray())
            .Select(n => (x0: n[0], y0: n[1], dx: n[2], dy: n[3]))
            .ToList();

        if (isPart1)
        {
            var final = robots.Select(n => (x: ((n.x0 + n.dx * seconds) % maxX + maxX) % maxX, y: ((n.y0 + n.dy * seconds) % maxY + maxY) % maxY)).ToList();
            return (final.Count(n => n.x < maxX / 2 && n.y < maxY / 2)
                 * final.Count(n => n.x < maxX / 2 && n.y > maxY / 2)
                 * final.Count(n => n.x > maxX / 2 && n.y < maxY / 2)
                 * final.Count(n => n.x > maxX / 2 && n.y > maxY / 2)).ToString();
        }

        for (var i = 0; i < maxX * maxY; i++)
        {
            var grid = new char[maxY][];
            for (var j = 0; j < maxY; j++)
                grid[j] = Enumerable.Repeat(' ', maxX).ToArray();

            robots.ForEach(r => grid[((r.y0 + r.dy * i) % maxY + maxY) % maxY][((r.x0 + r.dx * i) % maxX + maxX) % maxX] = '@');
            var lines = grid.Select(row => new string(row));
            if (lines.Count(line => line.Contains("@@@@@@")) > 9)
                return i.ToString();
        }
        return "???";
    }

    [GeneratedRegex(@"-?\d+")]
    private static partial Regex Integer { get; }
}
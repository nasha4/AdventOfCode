using System.Text.RegularExpressions;

namespace Advent_of_Code.Advent2024;

public partial class Day13(bool isPart1) : IAdventPuzzle
{
    public string Solve(InputHelper inputHelper)
    {
        var adjustment = isPart1 ? 0 : 10_000_000_000_000;
        return inputHelper.EachSection(lines => Integer().Matches(string.Join(" ", lines)))
            .Select(ms => ms.Select(m => long.Parse(m.Value)).ToArray())
            .Select(n => (x0: n[0], y0: n[1], x1: n[2], y1: n[3], xp: n[4] + adjustment, yp: n[5] + adjustment))
            .Sum(m =>
            {
                var d = m.y0 * m.x1 - m.x0 * m.y1;
                var da = m.y0 * m.xp - m.x0 * m.yp;
                var db = m.yp * m.x1 - m.xp * m.y1;
                return db % d == 0 && da % d == 0 ? 3 * db / d + da / d : 0;
            }).ToString();
    }

    [GeneratedRegex(@"-?\d+")]
    private static partial Regex Integer();
}

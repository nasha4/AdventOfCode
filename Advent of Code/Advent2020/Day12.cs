using Microsoft.VisualBasic;

namespace Advent_of_Code.Advent2020;

public class Day12(bool isPart1) : IAdventPuzzle
{
    public string Solve(InputHelper inputHelper)
    {
        var nav = inputHelper.EachLine(line => (act: line[0], val: int.Parse(line[1..])));

        var (x, y, _, _, _) = isPart1
            ? nav.Aggregate((x: 0, y: 0, f: 0, wx: 0, wy: 0), (pos, term) => (term.act, pos.f) switch
            {
                ('N', _) or ('F', 270) => pos with { y = pos.y + term.val },
                ('S', _) or ('F', 90) => pos with { y = pos.y - term.val },
                ('E', _) or ('F', 0) => pos with { x = pos.x + term.val },
                ('W', _) or ('F', 180) => pos with { x = pos.x - term.val },
                ('R', _) => pos with { f = (pos.f + term.val) % 360 },
                ('L', _) => pos with { f = (pos.f + 3 * term.val) % 360 },
                _ => pos
            })
            : nav.Aggregate((x: 0, y: 0, f: 0, wx: 10, wy: 1), (pos, term) => term switch
            {
                ('N', _) => pos with { wy = pos.wy + term.val },
                ('S', _) => pos with { wy = pos.wy - term.val },
                ('E', _) => pos with { wx = pos.wx + term.val },
                ('W', _) => pos with { wx = pos.wx - term.val },
                ('L', 90) or ('R', 270) => pos with { wx = -pos.wy, wy = pos.wx },
                ('L' or 'R', 180) => pos with { wx = -pos.wx, wy = -pos.wy },
                ('L', 270) or ('R', 90) => pos with { wx = pos.wy, wy = -pos.wx },
                ('F', _) => pos with { x = pos.x + pos.wx * term.val, y = pos.y + pos.wy * term.val },
                _ => pos
            });

        return $"{int.Abs(x) + int.Abs(y)}";
    }
}

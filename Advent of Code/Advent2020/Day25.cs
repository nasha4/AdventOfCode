namespace Advent_of_Code.Advent2020;

public class Day25(bool isPart1) : IAdventPuzzle
{
    public string Solve(InputHelper inputHelper)
    {
        var (keys, mod) = (inputHelper.EachLine(long.Parse).ToArray(), 20201227);
        for (var (v0, v1, vv) = (1L, 1L, 1L); ; (v0, v1, vv) = (v0 * keys[0] % mod, v1 * keys[1] % mod, vv * 7 % mod))
        {
            if (keys[0] == vv) return v1.ToString();
            if (keys[1] == vv) return v0.ToString();
        }
    }
}
using System.Text.RegularExpressions;

namespace Advent_of_Code.Advent2020;

public partial class Day14(bool isPart1) : IAdventPuzzle
{
    public string Solve(InputHelper inputHelper)
    {
        var memory = new Dictionary<long, long>();
        var (andMask, orMask, xMask) = (-1L, 0L, Array.Empty<long>());
        foreach (var groups in inputHelper.EachMatch(MemOrMask, match => match.Groups))
        {
            switch (groups)
            {
                case [_, _, _, { Success: true, Value: var mask }]:
                    andMask = mask.SelectMany((x, i) => x != '0' ? [35 - i] : Enumerable.Empty<int>()).Sum(n => 1L << n);
                    orMask = mask.SelectMany((x, i) => x == '1' ? [35 - i] : Enumerable.Empty<int>()).Sum(n => 1L << n);
                    xMask = mask.SelectMany((x, i) => x == 'X' ? [1L << (35 - i)] : Enumerable.Empty<long>()).ToArray();
                    break;
                case [_, { Value: var mem }, { Value: var val }, _] when isPart1:
                    memory[long.Parse(mem)] = long.Parse(val) & andMask | orMask;
                    break;
                case [_, { Value: var mem }, { Value: var val }, _]:
                    Enumerable.Range(0, 1 << xMask.Length) // Powerset
                        .Select(x => Enumerable.Range(0, xMask.Length).Where(y => ((1 << y) & x) > 0).Sum(i => xMask[i]))
                        .ToList().ForEach(xorMask => memory[(long.Parse(mem) | orMask) ^ xorMask] = long.Parse(val));
                    break;
            }
        }
        return memory.Values.Sum().ToString();
    }

    [GeneratedRegex(@"mem\[(\d+)\] = (\d+)|mask = (.+)")]
    private static partial Regex MemOrMask { get; }
}

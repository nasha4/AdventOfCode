using System.Text.RegularExpressions;

namespace Advent_of_Code.Advent2022;

public partial class Day05(bool isPart1) : IAdventPuzzle
{
    public string Solve(InputHelper inputHelper)
    {
        var grid = new GridHelper(inputHelper.EachLineInSection(line => line));
        var stacks = Enumerable.Range(0, (grid.Size[1] + 1) / 4)
            .Select(x => Enumerable.Range(0, grid.Size[0] - 1).Reverse().Select(y => grid[[y, x * 4 + 1]]).TakeWhile(c => c != ' ').ToList())
            .ToArray();

        foreach (var (n, from, to) in inputHelper.EachMatchGroup(Instruction(), match => match[1..].Select(int.Parse).ToArray()).Select(m => (m[0], m[1] - 1, m[2] - 1)))
        {
            stacks[to].AddRange(isPart1
                ? stacks[from].TakeLast(n).Reverse()
                : stacks[from].TakeLast(n));
            stacks[from].RemoveRange(stacks[from].Count - n, n);
        }
        return new string(stacks.Select(x => x[^1]).ToArray());
    }

    [GeneratedRegex(@"move (\d+) from (\d+) to (\d+)")]
    private static partial Regex Instruction();
}

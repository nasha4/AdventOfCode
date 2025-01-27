using System.Text.RegularExpressions;

namespace Advent_of_Code.Advent2024;

public partial class Day17(bool isPart1) : IAdventPuzzle
{
    public string Solve(InputHelper inputHelper)
    {
        var register = inputHelper.EachLineInSection(line => Integer.Match(line))
                .Select((m, i) => (register: i + 'A', value: int.Parse(m.Value)))
                .ToDictionary(t => (char)t.register, t => t.value);
        var instructions = inputHelper.EachLineInSection(line => line.Split(' ', ',').Skip(1).Select(int.Parse)).Single().ToArray();

        return isPart1
            ? string.Join(',', Run(instructions, register['A'], register['B'], register['C']))
            : UnRun(instructions, 0).ToString();
    }

    private static long UnRun(int[] instructions, long A)
    {
        if (instructions.Length == 0) return A;
        var decode = Enumerable.Range(0, 1 << 3)
            .Select(a => (A << 3) + a)
            // by examination and reverse engineering of my input instructions:
            .GroupBy(a => ((a >> (int)((a & 7) ^ 1)) ^ a ^ 5) & 7, a => a)
            .ToDictionary(g => g.Key, g => g.AsEnumerable());

        return decode.GetValueOrDefault(instructions[^1], [])
            .Select(a => UnRun(instructions[..^1], (A << 3) | a))
            .Where(x => x > -1)
            .Order()
            .FirstOrDefault(-1);
    }

    private static IEnumerable<int> Run(int[] instructions, long A, long B, long C)
    {
        for (var ip = 0; ip < instructions.Length; ip += 2)
        {
            var literal = instructions[ip + 1];
            var combo = (int)(literal switch { 4 => A, 5 => B, 6 => C, int x => x });
            switch (instructions[ip], A)
            {
                case (0, _): A >>= combo; break;
                case (1, _): B ^= literal; break;
                case (2, _): B = combo & 7; break;
                case (3, not 0): ip = literal - 2; break;
                case (4, _): B ^= C; break;
                case (5, _): yield return combo & 7; break;
                case (6, _): B = A >> combo; break;
                case (7, _): C = A >> combo; break;
            }
        }
    }

    [GeneratedRegex(@"-?\d+")]
    private static partial Regex Integer { get; }
}
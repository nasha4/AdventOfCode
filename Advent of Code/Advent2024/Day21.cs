using System.Text.RegularExpressions;

namespace Advent_of_Code.Advent2024;

public partial class Day21(bool isPart1) : IAdventPuzzle
{
    private sealed class RobotArm(Grid.Helper grid, Func<IReadOnlyDictionary<(char, char), long>, IReadOnlyDictionary<(char, char), long>>? presequencer = null)
    {
        private sealed class Orderer(string order) : IComparer<char>
        {
            public int Compare(char x, char y) => order.IndexOf(x).CompareTo(order.IndexOf(y));
        }
        private readonly IEnumerable<Orderer> orders = new[] { "^><v", "v<>^" }.Select(x => new Orderer(new string(x)));

        public IReadOnlyDictionary<(char, char), long> Sequences(IReadOnlyDictionary<(char, char), long> input) =>
            (presequencer is null ? input : presequencer(input))
                .SelectMany(kvp => ExpandBest(kvp.Key).ToDictionary(x => x.Key, x => x.Value * kvp.Value))
                .GroupBy(kvp => kvp.Key, kvp => kvp.Value)
                .ToDictionary(g => g.Key, g => g.Sum());

        private bool TouchesGap(char start, IEnumerable<char> moves)
        {
            var point = grid[start].Single().ToArray();
            var gaps = grid[' '];
            foreach (var m in moves)
            {
                if (gaps.Contains(point)) return true;
                switch (m)
                {
                    case '^': point[0]--; break;
                    case 'v': point[0]++; break;
                    case '<': point[1]--; break;
                    case '>': point[1]++; break;
                }
            }
            return false;
        }

        private IReadOnlyDictionary<(char, char), long> ExpandBest((char from, char to) trans)
        {
            return Expand(trans).MinBy(seq0 => seq0.Sum(Expand0))!;
            long Expand0(KeyValuePair<(char, char), long> t0) => t0.Value * Expand(t0.Key).Min(seq1 => seq1.Sum(Expand1));
            long Expand1(KeyValuePair<(char, char), long> t1) => t1.Value * Expand(t1.Key).Min(seq2 => seq2.Sum(Expand2));
            long Expand2(KeyValuePair<(char, char), long> t2) => t2.Value;
        }

        private IEnumerable<IReadOnlyDictionary<(char, char), long>> Expand((char from, char to) trans)
        {
            var dy = grid[trans.to].Single()[0] - grid[trans.from].Single()[0];
            var dx = grid[trans.to].Single()[1] - grid[trans.from].Single()[1];
            var sequences = orders.Select(o =>
                Enumerable.Repeat('^', dy < 0 ? -dy : 0)
                .Concat(Enumerable.Repeat('v', dy > 0 ? dy : 0))
                .Concat(Enumerable.Repeat('<', dx < 0 ? -dx : 0))
                .Concat(Enumerable.Repeat('>', dx > 0 ? dx : 0))
                .Order(o)
                .Append('B')
                .Prepend('B'));
            return sequences.Where(seq => !TouchesGap(trans.from, seq)).Select(seq =>
                seq.Zip(seq.Skip(1)).GroupBy(x => x).ToDictionary(g => g.Key, g => g.LongCount()) as IReadOnlyDictionary<(char, char), long>);
        }
    }

    public string Solve(InputHelper inputHelper)
    {
        var grid = new Grid.Helper(["789", "456", "123", " 0A", " ^B", "<v>"]);
        var robots = new List<RobotArm>() { new(grid) };
        for (var i = 0; i < 25; i++)
            robots.Add(new(grid, robots[i].Sequences));

        return inputHelper.EachLine(line => (
                numeric: long.Parse(Integer().Match(line).ValueSpan),
                transitions: line.Prepend('A').Zip(line).ToDictionary(x => x, _ => 1L)))
            .Sum(FinalScore).ToString();

        long FinalScore((long numeric, Dictionary<(char, char), long> transitions) x) =>
            robots[isPart1 ? 2 : ^1].Sequences(x.transitions).Values.Sum() * x.numeric;
    }

    [GeneratedRegex(@"-?\d+")]
    private static partial Regex Integer();
}

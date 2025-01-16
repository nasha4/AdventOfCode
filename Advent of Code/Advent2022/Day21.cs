using System.Text;
using System.Text.RegularExpressions;

namespace Advent_of_Code.Advent2022;

public partial class Day21(bool isPart1) : IAdventPuzzle
{
    private readonly record struct Algebraic
    {
        public readonly long? N { get; }
        public readonly Stack<(char op, long other)> Operations { get; init; }
        public Algebraic(long? n) => (N, Operations) = (n, new());
        private Algebraic(Algebraic copy) => (N, Operations) = (copy.N, new(copy.Operations.Reverse())); // Did you know Stacks copy in reverse? I do now!

        public Algebraic Operate(char op, Algebraic b)
        {
            switch (N, b.N, op)
            {
                case (null, null, _):
                    throw new ArgumentNullException(nameof(b), "Only one unknown supported");
                case (null, long bN, _):
                    var c = new Algebraic(this);
                    c.Operations.Push((op, bN));
                    return c;
                case (long aN, null, '+' or '*'):
                    c = new Algebraic(b);
                    c.Operations.Push((op, aN));
                    return c;
                case (long aN, null, '-'):
                    c = new Algebraic(b);
                    c.Operations.Push(('*', -1));
                    c.Operations.Push(('+', aN));
                    return c;
                case (long aN, long bN, '+'):
                    return new Algebraic(aN + bN);
                case (long aN, long bN, '-'):
                    return new Algebraic(aN - bN);
                case (long aN, long bN, '*'):
                    return new Algebraic(aN * bN);
                case (long aN, long bN, '/'):
                    return new Algebraic(aN / bN);
                case (_, _, _):
                    throw new NotSupportedException("operation not supported");
            }
        }
        public override string ToString()
        {
            if (N is not null) return N.Value.ToString();
            var sb = new StringBuilder("X");
            foreach (var (op, b) in Operations)
                sb.Append($" {op} {b}");
            return sb.ToString();
        }
        public static Algebraic Solve(Algebraic a, Algebraic b)
        {
            if (a.N is null && b.N is null)
                throw new ArgumentNullException(nameof(a), "Only one unknown supported");
            if (b.N is null)
                (a, b) = (b, a);
            var sum = new Algebraic(b);
            while (a.Operations.TryPop(out var pair))
            {
                var undo = pair.op switch
                {
                    '+' => '-',
                    '-' => '+',
                    '*' => '/',
                    '/' => '*',
                    _ => '!'
                };
                sum = sum.Operate(undo, new Algebraic(pair.other));
            }
            return sum;
        }
    }

    private readonly record struct Monkey(long? N, string A, string B, char Op);

    private Dictionary<string, Monkey> Monkeys { get; set; } = [];
    public string Solve(InputHelper inputHelper)
    {
        Monkeys = inputHelper.EachMatch(MonkeyPattern, m => m.Groups.Values.Select(g => g.Value).ToArray())
            .ToDictionary(m => m[1], m => new Monkey(m[2] == string.Empty ? null : long.Parse(m[2]), m[3], m[5], m[4] == string.Empty ? '!' : m[4][0]));

        return MonkeySolve("root").N.GetValueOrDefault().ToString();
    }

    private Algebraic MonkeySolve(string monkey) => (monkey, isPart1, Monkeys[monkey]) switch
    {
        ("humn", false, _) => new(null),
        (_, _, { N: long n }) => new(n),
        ("root", false, { A: string a, B: string b }) => Algebraic.Solve(MonkeySolve(a), MonkeySolve(b)),
        (_, _, { A: string a, B: string b, Op: char op }) => MonkeySolve(a).Operate(op, MonkeySolve(b))
    };

    [GeneratedRegex(@"(\w+): (?:(-?\d+)|(\w+) ([-+*/]) (\w+))")]
    private static partial Regex MonkeyPattern { get; }
}

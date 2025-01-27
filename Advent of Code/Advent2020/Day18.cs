using System.Text.RegularExpressions;

namespace Advent_of_Code.Advent2020;

public partial class Day18(bool isPart1) : IAdventPuzzle
{
    public string Solve(InputHelper inputHelper) => inputHelper.EachLine().Sum(isPart1 ? EvalLToR : EvalAddFirst).ToString();

    private static long EvalLToR(string expression) => Reduce(expression, EvalLToR)
        .Aggregate(0L, (total, term) => (term.op, term.n) switch { ("*", var n) => total * n, (_, var n) => total + n });

    private static long EvalAddFirst(string expression) => Reduce(expression, EvalAddFirst)
        .Aggregate(Enumerable.Empty<long>(), (adds, term) => (term.op, adds.LastOrDefault()) switch { ("+", long last) => [.. adds.SkipLast(1), term.n + last], _ => [.. adds, term.n] })
        .Aggregate((prod, term) => prod * term);

    private static IEnumerable<(string op, long n)> Reduce(string expression, Func<string, long> callBack) =>
        Tokens.Matches(expression).Prepend(Match.Empty).Chunk(2).Select(m => m switch {
            [var op, var v] when v.Groups["n"].Success => (op: op.Value, n: long.Parse(v.Value)),
            [var op, var v] => (op: op.Value, n: callBack(v.Value)),
            _ => throw new NotImplementedException()
        });

    [GeneratedRegex(@"(?<=\()(?>[^()]+|\((?<p>)|\)(?<-p>))*(?(p)(?!))(?=\))|[+*]|(?<n>\d+)", RegexOptions.ExplicitCapture)]
    private static partial Regex Tokens { get; }
}
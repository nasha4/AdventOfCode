using System.Text.RegularExpressions;

namespace Advent_of_Code.Advent2024;

public partial class Day24(bool isPart1) : IAdventPuzzle
{
    private Dictionary<string, bool> wires = [];
    private Dictionary<string, (string a, string b, string op)> gates = [];
    public string Solve(InputHelper inputHelper)
    {
        wires = inputHelper.EachLineInSection(line => line.Split(": ")).ToDictionary(x => x[0], x => x[1] == "1");
        gates = inputHelper.EachLineInSection(line => Gate().Match(line))
            .ToDictionary(x => x.Groups[4].Value, x => (a: x.Groups[1].Value, b: x.Groups[3].Value, op: x.Groups[2].Value));

        if (!isPart1)
        { // I found these by careful inspection and debugging, but they could probably be detected algorithmically
            (gates["z37"], gates["pqt"]) = (gates["pqt"], gates["z37"]);
            (gates["z09"], gates["cwt"]) = (gates["cwt"], gates["z09"]);
            (gates["z05"], gates["gdd"]) = (gates["gdd"], gates["z05"]);
            (gates["css"], gates["jmv"]) = (gates["jmv"], gates["css"]);
        }

        return gates.Where(g => g.Key.StartsWith('z')).OrderBy(z => z.Key).Select((z, i) => Calc(z.Key) ? 1L << i : 0).Sum().ToString();
    }

    private bool Calc(string wire) => wires.TryGetValue(wire, out var w) ? w
        : gates[wire].op switch {
            "XOR" => Calc(gates[wire].a) != Calc(gates[wire].b),
            "AND" => Calc(gates[wire].a) && Calc(gates[wire].b),
            _ => Calc(gates[wire].a) || Calc(gates[wire].b)
        };

    [GeneratedRegex(@"(...) (AND|OR|XOR) (...) -> (...)")]
    private static partial Regex Gate();
}

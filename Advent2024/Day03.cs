using System.Text.RegularExpressions;

namespace Advent_of_Code.Advent2024;

public partial class Day03(InputHelper inputHelper) : IAdventPuzzle
{
    public long Solve(bool isPart1)
    {
        var instructions = inputHelper.EachMatchGroup(Instruction(), groups =>
            string.IsNullOrWhiteSpace(groups[1])
                ? (instruction: "mul", a: int.Parse(groups[2]), b: int.Parse(groups[3]))
                : (instruction: groups[1], a: 0, b: 0));

        return isPart1
            ? instructions.Sum(x => x.a * x.b)
            : instructions.Aggregate((sum: 0, factor: 1),
                (current, term) => term.instruction switch
                {
                    "do" => (current.sum, 1),
                    "don't" => (current.sum, 0),
                    _ => (current.sum + term.a * term.b * current.factor, current.factor)
                }).sum;
    }

    [GeneratedRegex(@"(do|don't)\(\)|mul\((\d{1,3}),(\d{1,3})\)")]
    private static partial Regex Instruction();
}

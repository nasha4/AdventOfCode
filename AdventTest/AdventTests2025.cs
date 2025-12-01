using Advent_of_Code.Advent2025;

namespace AdventTest;

public class AdventSolutions2025 : AdventSolutions
{
    protected override Dictionary<Type, IEnumerable<object>> Solutions { get; } = new()
    {
        [typeof(Day01)] = [1129, 6638],
    };
}
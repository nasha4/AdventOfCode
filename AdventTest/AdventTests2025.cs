using Advent_of_Code.Advent2025;

namespace AdventTest;

public class AdventSolutions2025 : AdventSolutions
{
    protected override Dictionary<Type, IEnumerable<object>> Solutions { get; } = new()
    {
        [typeof(Day01)] = [1129, 6638],
        [typeof(Day02)] = [40214376723, 50793864718],
        [typeof(Day03)] = [17263, 170731717900423],
        [typeof(Day04)] = [1344, 8112],
        [typeof(Day05)] = [509, 336790092076620],
        [typeof(Day06)] = [5595593539811, 10153315705125],
        [typeof(Day07)] = [1553, 15811946526915],
        [typeof(Day08)] = [105952, 975931446],
    };
}